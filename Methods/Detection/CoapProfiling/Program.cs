using Accord.Statistics.Distributions.DensityKernels;
using Accord.Statistics.Distributions.Multivariate;
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
        static double windowSize = 10;
        static int windowCount = 20000;
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
                var records = csv.GetRecords<CoapRecord>();

                // extract information for computing window table:
                var items = records.Select(r => new CoapFeatures { StartMsec = r.StartMsec, SourceHost = $"{r.L3Ipv4Src}.{r.L4PortSrc}", CoapTarget = $"{ r.CoapCodeAlias}({r.CoapTypeAlias})[{r.CoapUriHost}{r.CoapUriPath}]" });
                var startMs = items.First().StartMsec;
                // split data into windows
                var windows = items.GroupBy(x => (int)Math.Floor((x.StartMsec - startMs) / windowSize)).Take(windowCount).ToArray();
                var samples = windows.Select(g => GetWindowSamples(g.ToArray())).ToArray();

                // get keys and assign indices
                var keySet = new HashSet<string>(samples.SelectMany(d => d.Keys));
                var columns = keySet.Select((x, i) => (Name:x,Index:i)).ToDictionary(x => x.Name);
                var observations = GetObservations(columns, samples).ToArray();

                IDensityKernel kernel = new EpanechnikovKernel(dimension: columns.Count);

                // Create a multivariate Empirical distribution from the observations
                var dist = new MultivariateEmpiricalDistribution(kernel, observations);


                // Common measures
                var mean = dist.Mean;     // { 3.71, 2.00 }
                var median = dist.Median; // { 3.71, 2.00 }
                var variance = dist.Variance;  // { 7.23, 5.00 } (diagonal from cov)
                var covariance = dist.Covariance; // { { 7.23, 0.83 }, { 0.83, 5.00 } }
            }
        }

        private static IEnumerable<double[]> GetObservations(Dictionary<string, (string Name, int Index)> columns, IDictionary<string, double>[] samples)
        {
            foreach(var sample in samples)
            {
                var row = new double[columns.Count];
                foreach(var val in sample)
                {
                    var idx = columns[val.Key].Index;
                    row[idx] = val.Value;
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
