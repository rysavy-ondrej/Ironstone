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
        internal static readonly double DefaultWindowSize = 60.0;

        public void Configuration(CommandLineApplication command)
        {
            command.Description = "Tests the differences of the communication to the CoAP profile.";
            command.HelpOption("-?|-Help");


            var windowOption = command.Option("-WindowSize <double>",
                "A size of windows in seconds.",
                CommandOptionType.MultipleValue);

            var readOption = command.Option("-InputFile <string>",
                "A name of an input file in csv format that contains decoded CoAP packets.",
                CommandOptionType.MultipleValue);

            var profileOption = command.Option("-ProfileFile <string>",
                "A name of a profile file.",
                CommandOptionType.MultipleValue);
            var modelKeyOption = command.Option("-ModelKey <string>",
                "Model key represents an aggregation scheme that is used to build individual flow models. It is usually drawn from the following values: 'coap.code', 'coap.type', 'coap.uri_path'. Default is  'coap.code,coap.type,coap.uri_path'.",
                CommandOptionType.SingleValue);

            var aggregateOption = command.Option("-Aggregate <string>",
                "Aggregation scheme enables to group flows to group of flows. It accepts any combination of 'ip.src', 'ip.dst', 'udp.srcport', 'udp.dstport'. Default is ''.",
                CommandOptionType.SingleValue);

            var tresholdOption = command.Option("-TresholdFactor <double>",
                "A Factor used to scale the treshold. The usual values are between 0-1. Default is 1.",
                CommandOptionType.SingleValue);

            command.OnExecute(() =>
            {
                var windowSize = windowOption.HasValue() ? Double.Parse(windowOption.Value()) : DefaultWindowSize;
                var flowAggregation = aggregateOption.HasValue() ? FieldHelper.Parse<FlowKey.Fields>(aggregateOption.Value(), (x, y) => (x | y)) : FlowKey.Fields.None;
                var modelKey = modelKeyOption.HasValue() ? FieldHelper.Parse<CoapResourceAccess.Fields>(modelKeyOption.Value(), (x, y) => (x | y)) : CoapResourceAccess.Fields.CoapCode | CoapResourceAccess.Fields.CoapType | CoapResourceAccess.Fields.CoapUriPath;
                var thresholdMultiplier = tresholdOption.HasValue() ? Double.Parse(tresholdOption.Value()) : 1;
                if (readOption.HasValue() && profileOption.HasValue())
                {                    
                    var profiles = profileOption.Values.Select(PrintProfile.LoadProfile).ToList();

                    var profile = profiles.Count == 1 ? profiles.First() : MergeProfiles(profiles);
                    profile.ThresholdMultiplier = thresholdMultiplier;
                    LoadAndTest(profile, readOption.Value(), windowSize, modelKey, flowAggregation);
                    return 0;
                }
                throw new CommandParsingException(command, $"Missing required arguments: {readOption.ShortName}, {profileOption.ShortName}.");
            });
        }

        private CoapProfile MergeProfiles(List<CoapProfile> profiles)
        {
            var first = profiles.First();

            var profile = new CoapProfile(first.Dimensions, first.WindowSize, first.ModelBuilder);
            var targets = profiles.SelectMany(p => p.Items).GroupBy(m => m.Key);
            foreach(var target in targets)
            {
                var model = profile.NewModel();
                foreach (var oldModel in target)
                {
                    model.Samples.AddRange(oldModel.Value.Samples);
                }
                profile.Add(target.Key, model);
            }
            profile.Commit();
            return profile;
        }

        private void LoadAndTest(CoapProfile profile, string inputFile, double windowSize, CoapResourceAccess.Fields modelKey, FlowKey.Fields flowAggregation = FlowKey.Fields.None) 
        {
            var getFlowKeyFunc = FlowKey.GetFlowKeyFunc(flowAggregation);
            var getModelKeyFunc = CoapResourceAccess.GetModelKeyFunc(modelKey);
            using (var reader = new StreamReader(inputFile))
            using (var csv = new CsvReader(reader))
            {
                csv.Configuration.PrepareHeaderForMatch = (string header, int index) => header.Trim();
                csv.Configuration.MissingFieldFound = null;
                csv.Configuration.HeaderValidated = null;
                var packets = csv.GetRecords<CoapPacketRecord>().ToList();
                var startTime = packets.First().TimeEpoch;
                var packetBins = packets.GroupBy(p => (int)Math.Floor((p.TimeEpoch - startTime) / windowSize));

                var normalTotal = 0;
                var abnormalTotal = 0;
                var unknownTotal = 0;

                foreach (var group in packetBins)
                {
                    var flows = CoapFlowRecord.CollectCoapFlows(group, getModelKeyFunc, getFlowKeyFunc);
                    var testResults = TestAndPrint(profile, flows, $"{DateTime.UnixEpoch.AddSeconds((long)startTime + (group.Key * windowSize))}");
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
        }

        private (int Normal, int Abnormal, int Unknown) TestAndPrint(CoapProfile profile, IEnumerable<CoapFlowRecord> flows, string label = "") 
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
                if (profile.TryGetValue(flow.CoapObject.ToString(), out var matchingProfile))
                {
                    var score = matchingProfile.Score(observation);
                    var matches = (score > (matchingProfile.Threshold * profile.ThresholdMultiplier));
                    matchingTable.Rows.Add($"{flow.Key} {flow.CoapObject}", flow.FlowPackets, flow.FlowOctets, score, matchingProfile.Threshold, matches ? "normal" : "abnormal");
                    if (matches) normalCount++;
                    else abnormalCount++;
                }
                else
                {
                    matchingTable.Rows.Add($"{flow.Key} {flow.CoapObject}", flow.FlowPackets, flow.FlowOctets, double.NaN, double.NaN, "unknown");
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