using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using ConsoleTableExt;
using CsvHelper;
using Microsoft.Extensions.CommandLineUtils;
using ShellProgressBar;

namespace Ironstone.Analyzers.CoapProfiling
{
    internal class LearnProfile : IDisposable
    {
        internal static readonly double DefaultWindowSize = 60.0;

        public LearnProfile()
        {

        }

        public void Configuration(CommandLineApplication command)
        {
            command.Description = "Create CoAP profiles from the specific capture file.";
            command.HelpOption("-?|-Help");

            var windowOption = command.Option("-WindowSize <double>",
                        "A size of windows in seconds.",
                        CommandOptionType.MultipleValue);

            var readOption = command.Option("-InputFile <filename.csv>",
                        "A name of input file in csv format that contains decoded CoAP packets.",
                        CommandOptionType.MultipleValue);

            var writeOption = command.Option("-WriteTo <filename>",
                        "An output file to write binary representation of the profile.",
                        CommandOptionType.SingleValue);

            command.OnExecute(() =>
            {
                double windowSize = windowOption.HasValue() ? Double.Parse(windowOption.Value()) : DefaultWindowSize;
                if (readOption.HasValue() && writeOption.HasValue())
                {
                    CreateProgressBar();
                    var profile = LoadAndCompute(readOption.Value(), windowSize);
                    StoreProfile(profile, writeOption.Value());
                    return 0;
                }
                throw new CommandParsingException(command, $"Missing required arguments: {readOption.ShortName}, {writeOption.ShortName}.");
            });
        }

        ProgressBar progressBar; 
        private void CreateProgressBar()
        {
            progressBar = new ProgressBar(0, String.Empty, new ProgressBarOptions
            {
                ProgressCharacter = '─',
            });
        }
        private void ResetProgressBar(int count, string message)
        {
            progressBar.Tick(0);
            progressBar.MaxTicks = count;
            progressBar.Message = message;
        }

        private void StoreProfile(CoapProfile<CoapStatisticalModel> profile, string outputFile)
        {
            var formatter = new BinaryFormatter();
            using (var s = new FileStream(outputFile, FileMode.Create))
            {
                formatter.Serialize(s, profile);
                s.Close();
            }
        }

        private CoapProfile<CoapStatisticalModel> LoadAndCompute(string inputFile, double windowSize)
        {
            var profile = new CoapProfile<CoapStatisticalModel>(windowSize);

            ResetProgressBar(0, $"Loading source packets...");
            using (var reader = new StreamReader(inputFile))
            using (var csv = new CsvReader(reader))
            {
                csv.Configuration.PrepareHeaderForMatch = (string header, int index) => header.Trim();
                csv.Configuration.MissingFieldFound = null;
                csv.Configuration.HeaderValidated = null;
                var packets = csv.GetRecords<CoapPacketRecord>().ToList();
                var startTime = packets.First().TimeEpoch;
                var packetBins = packets.GroupBy(p => (int)Math.Floor((p.TimeEpoch - startTime) / windowSize)).ToList();

                ResetProgressBar(packetBins.Count(), $"Processing bins...");
                foreach (var group in packetBins)
                {
                    progressBar.Tick();
                    var flows = CoapFlowRecord.CollectFlows(group, p => $"{p.IpSrc}.{p.SrcPort}->{p.IpDst}{p.DstPort}");
                    ComputeProfile(profile, flows);
                }
            }
            ResetProgressBar(profile.Count, $"Fitting models...");
            profile.Commit(() => progressBar.Tick());
            return profile;
        }


        private static void ComputeProfile(CoapProfile<CoapStatisticalModel> profile, IEnumerable<CoapFlowRecord> flows)
        {
            foreach (var flow in flows)
            {
                var coapObject = flow.CoapObject.ToString();
                if (!profile.TryGetValue(coapObject, out var model))
                {
                    model = new CoapStatisticalModel(2);
                    profile[coapObject] = model;
                }
                model.Samples.Add(new double[] { flow.FlowPackets, flow.FlowOctets });
            }
        }

        public void Dispose()
        {
            ((IDisposable)progressBar).Dispose();
        }
    }
}