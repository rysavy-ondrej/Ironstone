using Accord.Statistics.Distributions.DensityKernels;
using Accord.Statistics.Distributions.Fitting;
using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Testing;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CoapProfiling
{

    public class CoapRecord
    {
        [Name("Start Msec")]
        public double StartMsec { get; set; }
        [Name("L3 Ipv4 Src")]
        public string L3Ipv4Src     { get; set; }
        [Name("L4 Port Src")]
        public string L4PortSrc     { get; set; }
        [Name("L3 Ipv4 Dst")]
        public string L3Ipv4Dst     { get; set; }
        [Name("L4 Port Dst")]
        public string L4PortDst     { get; set; }
        [Name("Coap Code")]
        public int CoapCode      { get; set; }
        [Name("Coap Type")]
        public int CoapType      { get; set; }
        [Name("Coap Uri Host")]
        public string CoapUriHost   { get; set; }
        [Name("Coap Uri Path")]
        public string CoapUriPath   { get; set; }
        

        public string CoapCodeAlias
        {
            get {
                switch (CoapCode)
                {
                    case 0: return "NUL";
                    case 1: return "GET";
                    case 2: return "PUT";
                    case 3: return "DEL";
                    default:
                        return $"{(CoapCode >> 5)}.{(CoapCode & 0x1f):00}";
                }
            }
        }
        public string CoapTypeAlias
        {
            get
            {
                switch(CoapType)
                {
                    case 0: return "CON";
                    case 1: return "NON";
                    case 2: return "ACK";
                    case 3: return "RST";
                    default: return CoapType.ToString();
                }
            }
        }

    }


    public class CoapFeatures
    {
        [Name("Start Msec")]
        public double StartMsec { get; set; }

        [Name("Coap Target")]
        public string CoapTarget { get; set; }

        [Name("Source Host")]
        public string SourceHost { get; set; } 

    }

    class Program
    {

        static void Test()
        {
            double[][] observations =
            {
                new double[] { 1, 2 },
                new double[] { 1, 2 },
                new double[] { 2, 2 },
                new double[] { 1, 2 }
            };

            // Create a multivariate Gaussian for 2 dimensions
            var normal = new MultivariateNormalDistribution(2);
            // Specify a regularization constant in the fitting options
            NormalOptions options = new NormalOptions() { Regularization = regularization };

            // Fit the distribution to the data
            normal.Fit(observations, options);

            // Check distribution parameters
            double[] mean = normal.Mean;     // { 1, 2 }
            double[] var = normal.Variance; // { 4.9E-324, 4.9E-324 } (almost machine zero)

            var pdf1 = normal.ProbabilityDensityFunction(new double[] { 1,2 }) * regularization;
            var pdf2 = normal.ProbabilityDensityFunction(new double[] { 1.25,2 }) * regularization;
        }


        static double windowSize = 10;
        static int learningWindowCount = 200;
        static double regularization = 1e-10;
        static void Main(string[] args)
        {
            Test();
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: CoapProfiling <input-file.csv>");
                return;
            }
            var inputfile = args[0];
            using (var reader = new StreamReader(inputfile))
            using (var csv = new CsvReader(reader))
            {
                csv.Configuration.PrepareHeaderForMatch = (string header, int index) => header.Trim();
                var records = csv.GetRecords<CoapRecord>();

                // extract information for computing window table:
                var items = records.Select(r => new CoapFeatures { StartMsec = r.StartMsec, SourceHost = $"{r.L3Ipv4Src}.{r.L4PortSrc}", CoapTarget = $"{ r.CoapCodeAlias}({r.CoapTypeAlias})[{r.CoapUriHost}{r.CoapUriPath}]" });
                var startMs = items.First().StartMsec;
                // split data into windows
                var windows = items.GroupBy(x => (int)Math.Floor((x.StartMsec - startMs) / windowSize)).ToLookup(x => x.Key < learningWindowCount); 

                var learningSamples = windows[true].Select(g => GetWindowSamples(g.ToArray())).ToArray();

                // get keys and assign indices
                var columnNames = learningSamples.SelectMany(d => d.Keys).ToHashSet().Select((x, i) => (Name: x, Index: i)).ToDictionary(x => x.Name);

                IDensityKernel kernel = new EpanechnikovKernel(dimension: columnNames.Count);
                var observations = GetObservations(columnNames, learningSamples).ToArray();
                // var profileMED = new MultivariateEmpiricalDistribution(kernel, observations);
                var profileMED  = MultivariateNormalDistribution.Estimate(observations, new NormalOptions() { Regularization = regularization });

                var mean = profileMED.Mean;     
                var median = profileMED.Median; 
                var variance = profileMED.Variance;  
                var covariance = profileMED.Covariance; 


                var testSamples = windows[false].Select(g => GetWindowSamples(g.ToArray())).ToArray();
                var testObservations = GetObservations(columnNames, learningSamples).Select((x,i) => (sample: x, window: i));

                Console.WriteLine($"-Test Window------------------------------------------------------");
                foreach (var testObservation in testObservations)
                {
                    var pdf = profileMED.LogProbabilityDensityFunction(testObservation.sample);
                    Console.WriteLine($"Window:{testObservation.window}, Value: {pdf}");
                }
                Console.WriteLine($"-Random------------------------------------------------------");
                var random = new Random();
                for(int i = 0; i < 50; i++)
                {
                    var sample = new double[columnNames.Count + 1];
                    // random samples:
                    for (int j = 0; j< columnNames.Count; j++)
                    {
                        sample[j] = random.Next(100);
                    }
                    var pdf = profileMED.LogProbabilityDensityFunction(sample);
                    Console.WriteLine($"Random, Value: {pdf}");
                }
                Console.WriteLine($"-Injected-----------------------------------------------------");
                foreach (var testObservation in testObservations)
                {
                    var sample = new double[columnNames.Count + 1];
                    var inj = 0;
                    // random samples:
                    for (int j = 0; j <= columnNames.Count; j++)
                    {
                        if ((random.Next(100) == 50))
                        {
                            sample[j] = testObservation.sample[j] > 0 ? testObservation.sample[j] * 2 : mean[j];
                            inj++;
                        }
                        else
                        {
                            sample[j] = testObservation.sample[j];
                        }
                    }
                    var pdf = profileMED.LogProbabilityDensityFunction(sample);
                    Console.WriteLine($"Injected: {inj}, Value: {pdf}");
                }
            }
        }

        /// <summary>
        /// Get the observation matrix. 
        /// </summary>
        /// <param name="columns">Collection of columns.</param>
        /// <param name="samples">Collection of samples used to initiate observation matrix.</param>
        /// <returns>
        /// A collection of rows each containing a single observation within the window. 
        /// Each row has <paramref name="columns.Count"/>+1 items. The last item aggregates all values from 
        /// <paramref name="samples"/> which column name is not in <paramref name="columns"/> dictionary.
        /// </returns>
        private static IEnumerable<double[]> GetObservations(Dictionary<string, (string Name, int Index)> columns, IDictionary<string, double>[] samples)
        {
            foreach(var sample in samples)
            {
                var row = new double[columns.Count+1];
                foreach(var val in sample)
                {
                    if (columns.TryGetValue(val.Key, out var xx))
                        row[xx.Index] = val.Value;
                    else
                        row[columns.Count] += val.Value; 
                }
                yield return row;
            }
        }

        private static IDictionary<string,double> GetWindowSamples(CoapFeatures[] coapFeatures)
        {
            var t = coapFeatures.GroupBy(r => r.CoapTarget).Select(grp => new KeyValuePair<string, double>(grp.Key, grp.Count()));
            return new Dictionary<string, double>(t);
        }
    }
}
