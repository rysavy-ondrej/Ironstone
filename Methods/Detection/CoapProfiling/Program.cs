using Accord.Statistics.Analysis;
using Accord.Statistics.Distributions;
using Accord.Statistics.Distributions.DensityKernels;
using Accord.Statistics.Distributions.Fitting;
using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Distributions.Univariate;
using Accord.Statistics.Testing;
using ConsoleTableExt;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace CoapProfiling
{

    public class CoapRecord
    {
        [Name("Start Msec")]
        public double StartMsec     { get; set; }
        [Name("L3 Ipv4 Src")]
        public string L3Ipv4Src     { get; set; }
        [Name("L4 Port Src")]
        public string L4PortSrc     { get; set; }
        [Name("L3 Ipv4 Dst")]
        public string L3Ipv4Dst     { get; set; }
        [Name("L4 Port Dst")]
        public string L4PortDst     { get; set; }
        [Name("L4 Paylen")]
        public int L4Paylen         { get; set; }
        [Name("Coap Code")]
        public int CoapCode         { get; set; }
        [Name("Coap Type")]
        public int CoapType         { get; set; }
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

        [Name("Coap Object")]
        public string CoapObject { get; set; }

        [Name("Octets")]
        public int Octets { get; set; }

        [Name("Coap Uri Host")]
        public string CoapUriHost { get; set; } 

        [Name("Flow Key")]
        public string FlowKey { get; set; }

    }

    class DataItem
    {
        public double Packets { get; set; }
        public double Octets { get; set; }
        public double[] Observation => new[] { Packets, Octets };
        public override string ToString() => $"{{Packets={Packets},Octets={Octets}}}";
    }

    class Profile
    {
        static double regularization = 1e-10;
        static double epsilon = 0.001;
        static double thresholdMultipler = 2;
        public List<DataItem> Observations { get; } = new List<DataItem>();
        public IFittableDistribution<double> Distribution { get; private set;  }
        public double Pmax { get; private set; }
        public DistributionAnalysis Analysis { get; private set; }
        public double Threshold { get; internal set; }
        public double PacketsMean => (Distribution as EmpiricalDistribution).Mean;
        public double OctetsMean => 0;
        public double PacketsVariance => (Distribution as EmpiricalDistribution).Variance;
        public double OctetsVariance => 0; 

        double Amax;
        double Amin;
        /// <summary>
        /// Creates a new matrix for the given observations.
        /// </summary>
        /// <param name="observations">A list of two dimensional vectors. First dimension denotes number of packets while the second represents the total bytes.</param>
        /// <returns></returns>
        public MultivariateContinuousDistribution EstimateNormal()
        {
            // Create a multivariate Gaussian for 2 dimensions
            var distribution = new MultivariateNormalDistribution(2);
            // Specify a regularization constant in the fitting options
            var options = new NormalOptions() { Regularization = regularization };
            // Fit the distribution to the data
            distribution.Fit(Observations.Select(x => x.Observation).ToArray(), new NormalOptions
                {
                    Robust = true
                }
            );
            return distribution;
        }
        public void Learn()
        {
            var samples = Observations.Select(x => x.Packets).ToArray();

            var distribution = new EmpiricalDistribution(samples, 1/(double)samples.Length);
            //Distribution.Fit(samples);
            /*
            Analysis = new DistributionAnalysis();           
            var gof = Analysis.Learn(samples);
            Distribution = gof[0].Distribution;
            var maxPackets = (int)samples.Max();
            try
            {
                Distribution.ProbabilityFunction(maxPackets);
            }
            catch(Exception )
            {
                // if exception occured then select next one from the candidates...
                Distribution = gof[1].Distribution;
            }
            */
            Distribution = distribution;
            Pmax = samples.Select(distribution.ProbabilityDensityFunction).Max();
            Amin = 1; // 
            Amax = 1 / epsilon;
            
            // Compute threshold:
            var scores = Observations.Select(Score).ToList();
            var t_mean = scores.Average();
            var t_dev = scores.StdDev();
            var m2 = Math.Abs(distribution.Mean - distribution.Median);
            Threshold = ((t_mean + t_dev) == 0 ?  m2 : (t_mean + t_dev)) * thresholdMultipler;
        }

        public double Score(DataItem sample)
        {
            var pdf = Distribution.ProbabilityFunction(sample.Packets);
            var ai = 1 / Math.Max(epsilon, pdf/Pmax);
            var s = (ai - Amin)  // should be greater or equal to zero
                / (Amax - Amin);
            return s;
        }
    }


    class Program
    {
        static double windowSize = 10;
        static int learningWindowCount = 400;

        static void Main(string[] args)
        {
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
                csv.Configuration.MissingFieldFound = null;
                csv.Configuration.HeaderValidated = null;
                var records = csv.GetRecords<CoapRecord>();

                // extract information for computing window table:
                var items = records.Select(r => new CoapFeatures { StartMsec = r.StartMsec, FlowKey = $"{r.L3Ipv4Src}{r.L4PortSrc}->{r.L3Ipv4Dst}{r.L4PortDst}", CoapObject = $"{ r.CoapCodeAlias}({r.CoapTypeAlias})[{r.CoapUriPath}]", Octets = r.L4Paylen, CoapUriHost = r.CoapUriHost }).ToList();

                var start = items.First().StartMsec;
                var stop = items.Last().StartMsec;

                var profiles = ComputeProfiles(items.TakeWhile(x=> x.StartMsec < start + (windowSize * learningWindowCount)).ToList(), windowSize);

                var truePositive = 0;  // true positive is an outcome where the model correctly predicts the positive class.
                var trueNegative = 0;  // true negative is an outcome where the model correctly predicts the negative class
                var falsePositive = 0; // false positive is an outcome where the model incorrectly predicts the positive class.
                var falseNegative = 0; //  false negative is an outcome where the model incorrectly predicts the negative class

                // Test against the whole collection
                Console.WriteLine("SOURCE SAMPLES");
                foreach (var window in items.GroupBy(x => (int)Math.Floor((x.StartMsec - start) / windowSize)))
                {
                    var (pos,neg) = TestWindow(profiles, window);
                    truePositive += pos;
                    falseNegative += neg;
                }

                var items2 = RandomData(profiles, start, stop, items.Count()).ToList();
                Console.WriteLine("RANDOM SAMPLES");
                // Test against the randomly generated items
                foreach (var window in items2.GroupBy(x => (int)Math.Floor((x.StartMsec - start) / windowSize)))
                {
                    var (pos, neg) = TestWindow(profiles, window, "Random");
                    falsePositive += pos;
                    trueNegative += neg;
                }

                // Print confusion matrix:
                var confusionMatrix = new DataTable();
                confusionMatrix.Columns.Add("", typeof(string));
                confusionMatrix.Columns.Add("Normal Predicted", typeof(int));
                confusionMatrix.Columns.Add("Anomaly Predicted", typeof(int));
                confusionMatrix.Rows.Add("Normal Actual", truePositive, falseNegative);
                confusionMatrix.Rows.Add("Anomaly Actual", falsePositive, trueNegative);
                Console.WriteLine("Profiles:");
                ConsoleTableBuilder.From(confusionMatrix)
                   .WithFormat(ConsoleTableBuilderFormat.MarkDown)
                   .ExportAndWriteLine();
            }
        }

        private static IEnumerable<CoapFeatures> RandomData(IDictionary<string, Profile> profiles, double start, double stop, int count)
        {
            var interval = stop-start;
            var delta = interval / count;
            var random = new Random();
            var keys = profiles.Keys.ToArray();
            int GetOctets()
            {
                return 0;
            }

            for(int i=0; i < count; i++)
            {
                yield return new CoapFeatures { StartMsec = start + i * delta, CoapObject = keys[random.Next(keys.Length)], Octets = GetOctets() };
            }
        }

        private static (int Normal, int Anomaly) TestWindow(IDictionary<string, Profile> profiles, IGrouping<int, CoapFeatures> window, string label="")
        {
            var normal = 0;
            var anomaly = 0;
            var matchingTable = new DataTable();
            matchingTable.Columns.Add("CoAP Flow", typeof(string));
            matchingTable.Columns.Add("Packets", typeof(string));
            matchingTable.Columns.Add("Octets", typeof(string));
            matchingTable.Columns.Add("Score", typeof(double));
            matchingTable.Columns.Add("Threshold", typeof(double));
            matchingTable.Columns.Add("Anomaly", typeof(string));
            foreach (var (Key, Packets, Octets) in window.GroupBy(g => (Flow: g.FlowKey, Coap: g.CoapObject)).Select(g => (Key: g.Key, Packets: g.Count(), Octets: g.Sum(x => x.Octets))))
            {
                var observation = new DataItem { Packets = Packets, Octets = Octets };
                if (profiles.TryGetValue(Key.Coap, out var matchingProfile))
                {
                    var score = matchingProfile.Score(observation);
                    var isAnomalous = (score > matchingProfile.Threshold);
                    matchingTable.Rows.Add($"{Key.Flow} {Key.Coap}", observation.Packets, observation.Octets, score, matchingProfile.Threshold, isAnomalous.ToString());
                    if (isAnomalous) anomaly++; else normal++;
                }
                else
                {
                    matchingTable.Rows.Add(Key, double.NaN);
                }
            }
            Console.WriteLine($"{label} Window {window.Key}:");
            ConsoleTableBuilder.From(matchingTable)
                .WithFormat(ConsoleTableBuilderFormat.MarkDown)
                .ExportAndWriteLine();
            return (normal, anomaly);
        }

        private static IDictionary<string, Profile> ComputeProfiles(List<CoapFeatures> items, double windowsSize)
        {
            var startMs = items.First().StartMsec;
            // split data into windows
            var windows = items.GroupBy(x => (int)Math.Floor((x.StartMsec - startMs) / windowSize));            
            // get all Coap Resources in learning windows
            var profiles = windows.SelectMany(w => w.Select(r => r.CoapObject)).Distinct().ToDictionary(x => x, _ => new Profile());
            foreach (var window in windows)
            {
                foreach (var g in window.GroupBy(g => (Key: g.CoapObject,Flow: g.FlowKey)).Select(g => (Key: g.Key, Packets: g.Count(), Octets: g.Sum(x => x.Octets))))
                {
                    profiles[g.Key.Key].Observations.Add(new DataItem { Packets = g.Packets, Octets = g.Octets });
                }
            }
            var profilesTable = new DataTable();
            profilesTable.Columns.Add("Name", typeof(string));
            profilesTable.Columns.Add("Observations", typeof(int));
            profilesTable.Columns.Add("Distribution", typeof(string));
            profilesTable.Columns.Add("Mean.Packets", typeof(double));
            profilesTable.Columns.Add("Mean.Octets", typeof(double));
            profilesTable.Columns.Add("Variance.Packets", typeof(double));
            profilesTable.Columns.Add("Variance.Octets", typeof(double));
            profilesTable.Columns.Add("Threshold", typeof(double));
            foreach (var p in profiles)
            {
                p.Value.Learn();
                profilesTable.Rows.Add($"{p.Key}", p.Value.Observations.Count, p.Value.Distribution.ToString(), p.Value.PacketsMean, p.Value.OctetsMean, p.Value.PacketsVariance, p.Value.OctetsVariance, p.Value.Threshold);
            }

            Console.WriteLine("Profiles:");
            ConsoleTableBuilder.From(profilesTable)
               .WithFormat(ConsoleTableBuilderFormat.MarkDown)
               .ExportAndWriteLine();
            return profiles;
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
            var t = coapFeatures.GroupBy(r => r.CoapObject).Select(grp => new KeyValuePair<string, double>(grp.Key, grp.Count()));
            return new Dictionary<string, double>(t);
        }
    }
}
