using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using ConsoleTableExt;
using CsvHelper;
using Microsoft.Extensions.CommandLineUtils;

namespace Ironstone.Analyzers.CoapProfiling
{
    internal class TestCapture
    {
        public void Configuration(CommandLineApplication command)
        {
            command.Description = "Tests the differences of the communication to the CoAP profile.";
            command.HelpOption("-?|-Help");

            var windowOption = command.Option("-WindowSize <double>",
            "A size of windows in seconds.",
            CommandOptionType.MultipleValue);

            var readOption = command.Option("-InputFile <filename.csv>",
                        "A name of an input file in csv format that contains decoded CoAP packets.",
                        CommandOptionType.MultipleValue);

            var profileOption = command.Option("-ProfileFile <filename.profile>",
                        "A name of a profile file.",
                        CommandOptionType.MultipleValue);


            command.OnExecute(() =>
            {
                double windowSize = windowOption.HasValue() ? Double.Parse(windowOption.Value()) : LearnProfile.DefaultWindowSize;
                if (readOption.HasValue() && profileOption.HasValue())
                {
                    var profile = PrintProfile.LoadProfile(profileOption.Value());
                    LoadAndTest(profile, readOption.Value(), windowSize);
                    return 0;
                }
                throw new CommandParsingException(command, $"Missing required arguments: {readOption.ShortName}, {profileOption.ShortName}.");
            });
        }


        private void LoadAndTest(CoapProfile<CoapStatisticalModel> profile, string inputFile, double windowSize)
        {
            using (var reader = new StreamReader(inputFile))
            using (var csv = new CsvReader(reader))
            {
                csv.Configuration.PrepareHeaderForMatch = (string header, int index) => header.Trim();
                csv.Configuration.MissingFieldFound = null;
                csv.Configuration.HeaderValidated = null;
                var packets = csv.GetRecords<CoapPacketRecord>().ToList();
                var startTime = packets.First().TimeEpoch;
                var packetGroups = packets.GroupBy(p => (int)Math.Floor((p.TimeEpoch - startTime) / windowSize));
                foreach (var group in packetGroups)
                {
                    var flows = CoapFlowRecord.CollectFlows(group, p => $"{p.IpSrc}.{p.SrcPort}->{p.IpDst}{p.DstPort}");
                    TestAndPrint(profile, flows, $"{DateTime.UnixEpoch.AddSeconds((long)startTime + (group.Key * windowSize))}");
                }
            }
        }

        private (int Match, int Anomaly) TestAndPrint(CoapProfile<CoapStatisticalModel> profile, IEnumerable<CoapFlowRecord> flows, string label = "")
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
            foreach (var flow in flows)
            {
                var observation = new double[] { flow.FlowPackets, flow.FlowOctets };
                if (profile.TryGetValue(flow.CoapObject.ToString(), out var matchingProfile))
                {
                    var score = matchingProfile.Score(observation);
                    var isAnomalous = (score > (matchingProfile.Threshold));
                    matchingTable.Rows.Add($"{flow.FlowKey} {flow.CoapObject}", flow.FlowPackets, flow.FlowOctets, score, matchingProfile.Threshold, isAnomalous.ToString());
                    if (isAnomalous) anomaly++; else normal++;
                }
                else
                {
                    matchingTable.Rows.Add($"{flow.FlowKey} {flow.CoapObject}", flow.FlowPackets, flow.FlowOctets, double.NaN, double.NaN, "unknown");
                }
            }
            Console.WriteLine($"{label}:");
            ConsoleTableBuilder.From(matchingTable)
                .WithFormat(ConsoleTableBuilderFormat.MarkDown)
                .ExportAndWriteLine();
            return (normal, anomaly);
        }
    }
}