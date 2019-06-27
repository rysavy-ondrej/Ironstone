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
    public class CoapStatisticalModel : IFlowModel
    {
        const double epsilon = 1e-6;
        string[] m_dimensions;
        double[] m_pmax;

        public List<double[]> Samples { get; } = new List<double[]>();

        public UnivariateContinuousDistribution[] Distributions { get; private set;  }

        public double[] Mean => Distributions.Select(x => x.Mean).ToArray();

        public double[] Variance => Distributions.Select(x => x.Variance).ToArray();

        public double Threshold { get; internal set; }

        public Dictionary<string, string> Info => new Dictionary<string, string>
        {
            ["Distributions(Packets,Octets)"] = String.Join(',', Distributions.Select(x=>x.ToString())),
            ["Mean(Packets,Octets)"] = String.Join(',', Mean.Select(x=>x.ToString())),
            ["Variance(Packets,Octets)"] = String.Join(',', Variance.Select(x => x.ToString()))
        };

    private void _Initialize(string[] dimensions, double[][] samples)
        {
            m_dimensions = dimensions;
            m_pmax = new double[m_dimensions.Length];
            Distributions = new UnivariateContinuousDistribution[m_dimensions.Length];
            if (samples != null)
            {
                Samples.AddRange(samples);
            }
        }


        public CoapStatisticalModel(params string[] dimensions)
        {
            _Initialize(dimensions, null);   
        }

        public override string ToString()
        {
            var status = Distributions[0] == null ? "unfixed" : "learnt";
            return $"[dim={m_dimensions} samples={Samples.Count} status={status}]";
        }

        private UnivariateContinuousDistribution FitDistribution(double [] samples)
        {
            if (samples.Length < 10)
            {
                return new EmpiricalDistribution(samples);
            }
            else
            {
                var exp = new ExponentialDistribution(); // new LognormalDistribution(); // 
                exp.Fit(samples);
                return exp;
            }
        }

        private void FitByAnalysis()
        {
            for (int i = 0; i < m_dimensions.Length; i++)
            {
                var samples = Samples.Select(x => x[i]).ToArray();
                var distribution = FitDistribution(samples); 
                Distributions[i] = distribution;
                m_pmax[i] = samples.Select(s => GetProbability(distribution, s)).Max();
            }
        }

        /// <summary>
        /// Get the probability of occurence of <paramref name="value"/> in the given <paramref name="distribution"/>.
        /// The least not 0, but epsilon.
        /// <summary>
        double GetProbability(UnivariateContinuousDistribution distribution, double value)
        {
            try
            {
                var p = distribution.ProbabilityDensityFunction(value);
                return Math.Max(epsilon, p);
            }
            catch(InvalidOperationException)
            {
                return epsilon;
            }
        } 

        public void Fit()
        {
            FitByAnalysis();
            var scores = Samples.Select(s=> (Observation: s,Score: Score(s))).ToList();
            var (t_mean, t_dev) = scores.Select(s => s.Score).MeanAbsoluteDeviation();
            Threshold = Math.Max(0, t_mean - t_dev);
        }

        public double Score(double[] sample)
        {
            if (sample == null) throw new ArgumentNullException(nameof(sample));
            if (sample.Length < m_dimensions.Length) throw new ArgumentException("Provided array is shorter than a number of dimensions.");

            var a = new double[m_dimensions.Length];
            for (int i = 0; i < m_dimensions.Length; i++)
            {
                var p = GetProbability(Distributions[i], sample[i]);
                // we normalize the value to be between [0-1] as we compute the score as a product of individual probabilities:
                a[i] = p == m_pmax[i] ? 1 : p / m_pmax[i];
            }
            var s = a.Aggregate((x,y)=>x*y);
            return s;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("dimensions", m_dimensions, typeof(string[]));
            info.AddValue("samples", Samples.ToArray(), typeof(double[][]));
        }

        public CoapStatisticalModel(SerializationInfo info, StreamingContext context)
        {
            var dimensions =(string[]) info.GetValue("dimensions", typeof(string[]));
            var samples = (double[][])info.GetValue("samples", typeof(double[][]));
            _Initialize(dimensions, samples);
        }

    }
}
