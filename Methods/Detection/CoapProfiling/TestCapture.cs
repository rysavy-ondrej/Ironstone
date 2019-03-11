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

            var readOption = command.Option("-InputFile <filename.csv>",
                        "A name of an input file in csv format that contains decoded CoAP packets.",
                        CommandOptionType.MultipleValue);

            var profileOption = command.Option("-ProfileFile <filename.profile>",
                        "A name of a profile file.",
                        CommandOptionType.MultipleValue);
            var modelKeyOption = command.Option("-ModelKey <scheme>",
                "Model key represents an aggregation scheme that is used to build individual flow models. It is usually drawn from the following values: 'coap.code', 'coap.type', 'coap.uri_path'. Default is  'coap.code,coap.type,coap.uri_path'.",
                CommandOptionType.SingleValue);

            var aggregateOption = command.Option("-Aggregate <scheme>",
                "Aggregation scheme enables to group flows to group of flows. It accepts any combination of 'ip.src', 'ip.dst', 'udp.srcport', 'udp.dstport'. Default is ''.",
                CommandOptionType.SingleValue);

            command.OnExecute(() =>
            {
                var windowSize = windowOption.HasValue() ? Double.Parse(windowOption.Value()) : DefaultWindowSize;
                var flowAggregation = aggregateOption.HasValue() ? FieldHelper.Parse<FlowKey.Fields>(aggregateOption.Value(), (x, y) => (x | y)) : FlowKey.Fields.None;
                var modelKey = modelKeyOption.HasValue() ? FieldHelper.Parse<CoapResourceAccess.Fields>(modelKeyOption.Value(), (x, y) => (x | y)) : CoapResourceAccess.Fields.CoapCode | CoapResourceAccess.Fields.CoapType | CoapResourceAccess.Fields.CoapUriPath;

                if (readOption.HasValue() && profileOption.HasValue())
                {
                    var profile = PrintProfile.LoadProfile(profileOption.Value());
                    LoadAndTest(profile, readOption.Value(), windowSize, modelKey, flowAggregation);
                    return 0;
                }
                throw new CommandParsingException(command, $"Missing required arguments: {readOption.ShortName}, {profileOption.ShortName}.");
            });
        }


        private void LoadAndTest(CoapProfile<CoapStatisticalModel> profile, string inputFile, double windowSize, CoapResourceAccess.Fields modelKey, FlowKey.Fields flowAggregation = FlowKey.Fields.None)
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

                var matched = 0;
                var unmatched = 0;
                var unknown = 0;

                foreach (var group in packetBins)
                {
                    var flows = CoapFlowRecord.CollectCoapFlows(group, getModelKeyFunc, getFlowKeyFunc);
                    var testResults = TestAndPrint(profile, flows, $"{DateTime.UnixEpoch.AddSeconds((long)startTime + (group.Key * windowSize))}");
                    matched += testResults.Matched;
                    unmatched += testResults.Unmatched;
                    unknown += testResults.Unknonwn;
                }
                var measuresTable = new DataTable();
                measuresTable.Columns.Add("Metric", typeof(string));
                measuresTable.Columns.Add("Value", typeof(double));
                measuresTable.Rows.Add("Matched", matched);
                measuresTable.Rows.Add("Unmatched", unmatched);
                measuresTable.Rows.Add("Unknown", unknown);
                Console.WriteLine("Measures:");
                ConsoleTableBuilder.From(measuresTable)
                    .WithFormat(ConsoleTableBuilderFormat.MarkDown)
                    .ExportAndWriteLine();
            }
        }

        private (int Matched, int Unmatched, int Unknonwn) TestAndPrint(CoapProfile<CoapStatisticalModel> profile, IEnumerable<CoapFlowRecord> flows, string label = "")
        {
            var matched = 0;
            var unmatched = 0;
            var unknown = 0;
            var matchingTable = new DataTable();
            matchingTable.Columns.Add("CoAP Flow", typeof(string));
            matchingTable.Columns.Add("Packets", typeof(string));
            matchingTable.Columns.Add("Octets", typeof(string));
            matchingTable.Columns.Add("Score", typeof(double));
            matchingTable.Columns.Add("Threshold", typeof(double));
            matchingTable.Columns.Add("Matches?", typeof(string));
            foreach (var flow in flows)
            {
                var observation = new double[] { flow.FlowPackets, flow.FlowOctets };
                if (profile.TryGetValue(flow.CoapObject.ToString(), out var matchingProfile))
                {
                    var score = matchingProfile.Score(observation);
                    var matches = (score <= (matchingProfile.Threshold));
                    matchingTable.Rows.Add($"{flow.Key} {flow.CoapObject}", flow.FlowPackets, flow.FlowOctets, score, matchingProfile.Threshold, matches.ToString());
                    if (matches) matched++;
                    else unmatched++;
                }
                else
                {
                    matchingTable.Rows.Add($"{flow.Key} {flow.CoapObject}", flow.FlowPackets, flow.FlowOctets, double.NaN, double.NaN, "unknown");
                    unknown++;
                }
            }
            Console.WriteLine($"{label}:");
            ConsoleTableBuilder.From(matchingTable)
                .WithFormat(ConsoleTableBuilderFormat.MarkDown)
                .ExportAndWriteLine();
            return (matched, unmatched, unknown);
        }

        void PrintEvaluation(int truePositive, int trueNegative, int falsePositive, int falseNegative)
        {
            var contingencyTable = new DataTable();
            contingencyTable.Columns.Add("", typeof(string));
            contingencyTable.Columns.Add("Normal Predicted", typeof(int));
            contingencyTable.Columns.Add("Anomaly Predicted", typeof(int));
            contingencyTable.Rows.Add("Normal Actual", truePositive, falseNegative);
            contingencyTable.Rows.Add("Anomaly Actual", falsePositive, trueNegative);
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