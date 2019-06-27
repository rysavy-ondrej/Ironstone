using ConsoleTableExt;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Ironstone.Analyzers.CoapProfiling
{
    internal class TestCapture
    {
        internal static readonly double DefaultWindowSize = 60.0;

        public void Configuration(CommandLineApplication command)
        {
            command.Description = "Tests the differences of the communication to the CoAP profile.";
            command.HelpOption("-?|-Help");

            var inputCsvOption = command.Option("-InputCsvFile <string>",
                "A name of an input file in csv format that contains decoded CoAP packets.",
                CommandOptionType.MultipleValue);

            var inputCapOption = command.Option("-InputCapFile <string>",
                "A name of an input file in cap format that contains CoAP packets.",
                CommandOptionType.MultipleValue);

            var profileOption = command.Option("-ProfileFile <string>",
                "A name of a profile file.",
                CommandOptionType.MultipleValue);

            var tresholdOption = command.Option("-TresholdFactor <double>",
                "A Factor used to scale the treshold. The usual values are between 0-1. Default is 1.",
                CommandOptionType.SingleValue);

            command.OnExecute(() =>
            {
               var thresholdMultiplier = tresholdOption.HasValue() ? Double.Parse(tresholdOption.Value()) : 1;
                if (profileOption.HasValue())
                {                    
                    var profiles = profileOption.Values.Select(PrintProfile.LoadProfile).ToList();
                    var profile = profiles.Count == 1 ? profiles.First() : FlowProfile.MergeProfiles(profiles);
                    profile.ThresholdMultiplier = thresholdMultiplier;
                    if (inputCsvOption.HasValue())
                    {
                        var packets = PacketLoader.LoadCoapPacketsFromCsv(inputCsvOption.Value());
                        LoadAndTest(profile, packets); 
                    }
                    if (inputCapOption.HasValue())
                    {
                        var packets = PacketLoader.LoadCoapPacketsFromCap(inputCapOption.Value());
                        LoadAndTest(profile, packets);
                    }
                    return 0;
                }
                throw new CommandParsingException(command, $"Missing required arguments: {inputCsvOption.ShortName}, {profileOption.ShortName}.");
            });
        }
        private void LoadAndTest(FlowProfile profile, IEnumerable<IPacketRecord> packets)
        {
            var getFlowKeyFunc = FlowKey.GetFlowKeyFunc(profile.FlowAggregation);
            var getModelKeyFunc = profile.ProtocolFactory.GetModelKeyFunc(profile.ModelKey);

            var startTime = packets.First().TimeEpoch;
            var packetBins = packets.GroupBy(p => (int)Math.Floor((p.TimeEpoch - startTime) / profile.WindowSize));

            var normalTotal = 0;
            var abnormalTotal = 0;
            var unknownTotal = 0;

            foreach (var group in packetBins)
            {
                var flows = profile.ProtocolFactory.CollectCoapFlows(group, getModelKeyFunc, getFlowKeyFunc);
                var testResults = TestAndPrint(profile, flows, $"{DateTime.UnixEpoch.AddSeconds((long)startTime + (group.Key * profile.WindowSize))}");
                normalTotal += testResults.Normal;
                abnormalTotal += testResults.Abnormal;
                unknownTotal += testResults.Unknown;
            }
            var measuresTable = new DataTable();
            measuresTable.Columns.Add("Metric", typeof(string));
            measuresTable.Columns.Add("Value", typeof(double));
            measuresTable.Rows.Add("Normal", normalTotal);
            measuresTable.Rows.Add("Abnormal", abnormalTotal);
            measuresTable.Rows.Add("Unknown", unknownTotal);
            Console.WriteLine("Measures:");
            ConsoleTableBuilder.From(measuresTable)
                .WithFormat(ConsoleTableBuilderFormat.MarkDown)
                .ExportAndWriteLine();

        }

        private (int Normal, int Abnormal, int Unknown) TestAndPrint(FlowProfile profile, IEnumerable<IFlowRecord> flows, string label = "") 
        {
            var normalCount = 0;
            var abnormalCount = 0;
            var unknownCount = 0;
            var matchingTable = new DataTable();
            matchingTable.Columns.Add("CoAP Flow", typeof(string));
            matchingTable.Columns.Add("Packets", typeof(string));
            matchingTable.Columns.Add("Octets", typeof(string));
            matchingTable.Columns.Add("Score", typeof(double));
            matchingTable.Columns.Add("Threshold", typeof(double));
            matchingTable.Columns.Add("Label", typeof(string));
            foreach (var flow in flows)
            {
                var observation = new double[] { flow.FlowPackets, flow.FlowOctets };
                if (profile.TryGetValue(flow.Model.ToString(), out var matchingProfile))
                {
                    var score = matchingProfile.Score(observation);
                    var matches = (score > (matchingProfile.Threshold * profile.ThresholdMultiplier));
                    matchingTable.Rows.Add($"{flow.Key} {flow.Model}", flow.FlowPackets, flow.FlowOctets, score, matchingProfile.Threshold, matches ? "normal" : "abnormal");
                    if (matches) normalCount++;
                    else abnormalCount++;
                }
                else
                {
                    matchingTable.Rows.Add($"{flow.Key} {flow.Model}", flow.FlowPackets, flow.FlowOctets, double.NaN, double.NaN, "unknown");
                    unknownCount++;
                }
            }
            Console.WriteLine($"{label}:");
            ConsoleTableBuilder.From(matchingTable)
                .WithFormat(ConsoleTableBuilderFormat.MarkDown)
                .ExportAndWriteLine();
            return (normalCount, abnormalCount, unknownCount);
        }

        void PrintEvaluation(int truePositive, int trueNegative, int falsePositive, int falseNegative)
        {
            var contingencyTable = new DataTable();
            contingencyTable.Columns.Add("", typeof(string));
            contingencyTable.Columns.Add("Normal Predicted", typeof(int));
            contingencyTable.Columns.Add("Abnormal Predicted", typeof(int));
            contingencyTable.Rows.Add("Normal Actual", truePositive, falseNegative);
            contingencyTable.Rows.Add("Abnormal Actual", falsePositive, trueNegative);
            Console.WriteLine("Confusion Matrix:");
            ConsoleTableBuilder.From(contingencyTable)
                .WithFormat(ConsoleTableBuilderFormat.MarkDown)
                .ExportAndWriteLine();

            // compute precision
            var precision = truePositive / (double)(truePositive + falsePositive);
            var recall = truePositive / (double)(truePositive + falseNegative);
            var accuracy = (truePositive + trueNegative) / (double)(truePositive + trueNegative + falsePositive + falseNegative);
            var fmeasure = 2 * (precision * recall) / (precision + recall);
            var measuresTable = new DataTable();
            measuresTable.Columns.Add("Metric", typeof(string));
            measuresTable.Columns.Add("Value", typeof(double));
            measuresTable.Rows.Add("Precision", precision);
            measuresTable.Rows.Add("Recall", recall);
            measuresTable.Rows.Add("Accuracy", accuracy);
            measuresTable.Rows.Add("F-measure", fmeasure);
            Console.WriteLine("Measures:");
            ConsoleTableBuilder.From(measuresTable)
                .WithFormat(ConsoleTableBuilderFormat.MarkDown)
                .ExportAndWriteLine();
        }
    }
}