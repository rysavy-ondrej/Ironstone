using Accord.Statistics.Analysis;
using Accord.Statistics.Distributions;
using Accord.Statistics.Distributions.Fitting;
using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Distributions.Univariate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Ironstone.Analyzers.CoapProfiling.Models
{
    [Serializable]
    class CoapStatisticalFingerprint : IFlowModel
    {
        const double epsilon = 0.001;
        const double Amax = 1 / epsilon;
        const double Amin = 1;
        int m_dimension;
        double[] m_pmax;

        public List<double[]> Samples { get; } = new List<double[]>();

        public IFittableDistribution<double>[] Distributions { get; private set;  }

        public double[] Mean => Distributions.Select(x => ((x as UnivariateContinuousDistribution)?.Mean) ?? ((x as UnivariateDiscreteDistribution)?.Mean) ?? double.NaN).ToArray();

        public double[] Variance => Distributions.Select(x => ((x as UnivariateContinuousDistribution)?.Variance) ?? ((x as UnivariateDiscreteDistribution)?.Variance) ?? double.NaN).ToArray();

        public double Threshold { get; internal set; }

        public Dictionary<string, string> Info => new Dictionary<string, string>
        {
            ["Distributions(Packets,Octets)"] = String.Join(',', Distributions.Select(x=>x.ToString())),
            ["Mean(Packets,Octets)"] = String.Join(',', Mean.Select(x=>x.ToString())),
            ["Variance(Packets,Octets)"] = String.Join(',', Variance.Select(x => x.ToString()))
        };

    private void _Initialize(int dimension, double[][] samples)
        {
            m_dimension = dimension;
            m_pmax = new double[m_dimension];
            Distributions = new IFittableDistribution<double>[m_dimension];
            if (samples != null)
            {
                Samples.AddRange(samples);
            }
        }


        public CoapStatisticalFingerprint(string[] dimensions)
        {
            _Initialize(dimensions.Length, null);   
        }

        public override string ToString()
        {
            var status = Distributions[0] == null ? "unfixed" : "learnt";
            return $"[dim={m_dimension} samples={Samples.Count} status={status}]";
        }

        private void FitByAnalysis()
        {
            for (int i = 0; i < m_dimension; i++)
            {
                var samples = Samples.Select(x => x[i]).ToArray();
                var da = new DistributionAnalysis();
                // Learn the analysis
                var fit = da.Learn(samples);
                var distribution = fit[0].Distribution;
                Distributions[i] = distribution;
                m_pmax[i] = samples.Select(s => { try { return distribution.ProbabilityFunction(s); }  catch(Exception) { return 0; } }).Max();
            }
        }


        public void Fit()
        {
            FitByAnalysis();
            // FitEmpirical();
            var scores = Samples.Select(Score).ToList();
            var t_mean = scores.Average();
            var t_dev = scores.StdDev();
            Threshold = Math.Sqrt(t_mean + t_dev);
        }

        /// <summary>
        /// Fit the model to actual set of samples.
        /// </summary>
        private void FitEmpirical()
        {
            for (int i = 0; i < m_dimension; i++)
            {
                var samples = Samples.Select(x => x[i]).ToArray();
                var distribution = new EmpiricalDistribution(samples, 1 / (double)samples.Length);
                Distributions[i] = distribution;
                m_pmax[i] = samples.Select(distribution.ProbabilityDensityFunction).Max();                
            }
        }

        public double Score(double[] sample)
        {
            if (sample == null) throw new ArgumentNullException(nameof(sample));
            if (sample.Length < m_dimension) throw new ArgumentException("Provided array is shorter than number of dimensions.");
            var a = new double[m_dimension];
            for (int i = 0; i < m_dimension; i++)
            {
                var pdf = 0.0;
                try
                {
                    pdf = Distributions[i].ProbabilityFunction(sample[i]);
                }
                catch(Exception e)
                { }

                a[i] = 1 / Math.Max(epsilon, pdf == m_pmax[i] ? 1 : pdf / m_pmax[i]);
            }
            var s = (a.Average() - Amin) / (Amax - Amin);
            return s;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("dimension", m_dimension, typeof(int));
            info.AddValue("samples", Samples.ToArray(), typeof(double[][]));
        }

        public CoapStatisticalFingerprint(SerializationInfo info, StreamingContext context)
        {
            var dimension = info.GetInt32("dimension");
            var samples = (double[][])info.GetValue("samples", typeof(double[][]));
            _Initialize(dimension, samples);
        }

    }
}
