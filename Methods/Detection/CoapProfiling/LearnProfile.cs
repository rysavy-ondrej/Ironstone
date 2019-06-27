using Ironstone.Analyzers.CoapProfiling.Models;
using Konsole;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace Ironstone.Analyzers.CoapProfiling
{



    internal class LearnProfile
    {
        internal static readonly double DefaultWindowSize = 60.0;


        struct Settings
        {
            public double WindowSize { get; internal set; }
            public string Protocol { get; internal set; }
            public string FlowAggregation { get; internal set; }
            public string ModelKey { get; internal set; }
            public string ModelClass { get; internal set; }
            public LearningWindow WindowsCount { get; internal set; }
            public FlowKey.Fields FlowAggregationFields => FieldHelper.Parse<FlowKey.Fields>(FlowAggregation, (x, y) => (x | y));

            public Type ModelType
            {
                get
                {
                    var types = Assembly.GetExecutingAssembly().GetTypes(); // (ModelClass, true, true);
                    var className = this.ModelClass;
                    return types.FirstOrDefault(x => x.Name == className);
                }
            }

        }

        public LearnProfile()
        {

        }

        public void Configuration(CommandLineApplication command)
        {
            command.Description = "Create CoAP profiles from the specific capture file.";
            command.HelpOption("-?|-Help");

            var inputCsvOption = command.Option("-InputCsvFile <string>",
                "A name of an input file in csv format that contains decoded CoAP packets.",
                CommandOptionType.MultipleValue);

            var inputCapOption = command.Option("-InputCapFile <string>",
                "A name of an input file in cap format that contains CoAP packets.",
                CommandOptionType.MultipleValue);

            var writeOption = command.Option("-WriteTo <filename>",
                "An output file to write binary representation of the profile.",
                CommandOptionType.SingleValue);

            var windowSizeOption = command.Option("-WindowSize <double>",
                "A size of windows in seconds.",
                CommandOptionType.MultipleValue);

            var protocolOption = command.Option("-Protocol <string>",
                "Specifies the protocol to analyze. Supported protocolas are: coap, iec. Default is coap.",
                CommandOptionType.SingleValue);

            var modelKeyOption = command.Option("-ModelKey <scheme>",
                "Model key represents an aggregation scheme that is used to build individual flow models. It is usually drawn from the following values: 'coap.code', 'coap.type', 'coap.uri_path'. Default is  'coap.code,coap.type,coap.uri_path'.",
                CommandOptionType.SingleValue);

            var aggregateOption = command.Option("-Aggregate <scheme>",
                "Aggregation scheme enables to group flows to group of flows. It accepts any combination of 'ip.src', 'ip.dst', 'udp.srcport', 'udp.dstport'. Default is ''.",
                CommandOptionType.SingleValue);

            var modelClassOption = command.Option("-ModelClass <string>",
                "A name of class representing the model.",
                CommandOptionType.SingleValue);

            var windowsCountOption = command.Option("-LearningWindows <double[%]>",
                "A number of windows used to learn from the source data. Default is all. It is possible to specify a ratio of data used for learning, e.g., '20%'.",
                CommandOptionType.SingleValue);


            command.OnExecute(() =>
            {
                if (!writeOption.HasValue()) throw new CommandParsingException(command, $"{writeOption.ShortName} is required but was not provided!");
                if (!(inputCsvOption.HasValue() || inputCapOption.HasValue())) throw new CommandParsingException(command, $"{inputCsvOption.ShortName} or {inputCapOption.ShortName} is required but was not provided!");

                var settings = new Settings
                {
                    WindowSize = windowSizeOption.HasValue() ? Double.Parse(windowSizeOption.Value()) : DefaultWindowSize,
                    Protocol = protocolOption.HasValue() ? protocolOption.Value() : "coap",
                    FlowAggregation = aggregateOption.HasValue() ? aggregateOption.Value() : String.Empty,
                    ModelKey = modelKeyOption.HasValue() ? modelKeyOption.Value() : String.Empty,
                    ModelClass = modelClassOption.HasValue()? modelKeyOption.Value() : nameof(CoapStatisticalModel),
                    WindowsCount = windowsCountOption.HasValue() ? LearningWindow.Parse(windowsCountOption.Value()) : LearningWindow.All
                };

                ProcessInput(inputCsvOption.Value(), inputCapOption.Value(), writeOption.Value(), settings);
                return 0;
            });
        }

        private void ProcessInput(string inputCsvFile, string inputCapFile, string outputFile, Settings settings)
        {

            var flowAggregation = settings.FlowAggregationFields;
            var protocolFactory = ProtocolFactory.Create(settings.Protocol);
            var modelKey = protocolFactory.GetModelKeyFields(settings.ModelKey);

            Console.WriteLine("IRONSTONE FLOW PROFILING: Learn Profile");
            var profile = ProfileFactory.Create(protocolFactory, flowAggregation, modelKey, settings.ModelType, new string[] { "Packets", "Octets" }, settings.WindowSize);

            if (File.Exists(inputCsvFile))
            {
                var packets = PacketLoader.LoadCoapPacketsFromCsv(inputCsvFile).ToList();
                LoadAndCompute(profile, Path.GetFullPath(inputCsvFile), packets, settings.WindowSize, settings.WindowsCount, protocolFactory, modelKey, flowAggregation);
            }
            if (File.Exists(inputCapFile))
            {
                var packets = PacketLoader.LoadCoapPacketsFromCap(inputCapFile).ToList();
                LoadAndCompute(profile, Path.GetFullPath(inputCapFile), packets, settings.WindowSize, settings.WindowsCount, protocolFactory, modelKey, flowAggregation);
            }
            StoreProfile(profile, outputFile);
        }

        private void StoreProfile(FlowProfile profile, string outputFile)
        {
            var fullPath = Path.GetFullPath(outputFile);
            var sw = new Stopwatch();
            sw.Start();
            Console.WriteLine("┌ Saving profile:");
            var formatter = new BinaryFormatter();
            using (var s = new FileStream(outputFile, FileMode.Create))
            {
                formatter.Serialize(s, profile);
                s.Close();
            }
            Console.WriteLine($"├─ profile written to {fullPath}");
            Console.WriteLine($"└ done [{sw.Elapsed}].");
        }

        private void LoadAndCompute(FlowProfile profile, string inputFile,  IList<CoapPacketRecord> packets, double windowSize, LearningWindow learningWindows, 
            ProtocolFactory protocolFactory, Enum modelKey, FlowKey.Fields flowAggregation = FlowKey.Fields.None) 
        {
            var getFlowKeyFunc = FlowKey.GetFlowKeyFunc(flowAggregation);
            var getModelKeyFunc = protocolFactory.GetModelKeyFunc(modelKey);
            Console.WriteLine ("┌ Load packets and compute profile:");
            Console.WriteLine($"├─ input file: {inputFile}");
            Console.WriteLine($"├─ packets count: {packets.Count}");
            Console.WriteLine($"├─ window size: {windowSize} seconds");
            Console.WriteLine($"├─ learning window: {learningWindows.Meassure}({learningWindows.Value})");
            Console.WriteLine($"├─ protocol: {protocolFactory.Name}");
            Console.WriteLine($"├─ model key: {modelKey}");
            Console.WriteLine($"├─ flow aggregation: {flowAggregation}");
            var sw = new Stopwatch();
            sw.Start();

            var startTime = packets.First().TimeEpoch;
            var packetBins = packets.GroupBy(p => (int)Math.Floor((p.TimeEpoch - startTime) / windowSize)).ToList();

            var learningBins = learningWindows.Meassure == LearningWindow.ValueType.Absolute ? packetBins.Take((int)learningWindows.Value) :
                packetBins.Take((int)(packetBins.Count() * learningWindows.Value));
            var pb1 = new ProgressBar(packetBins.Count());
            foreach (var group in packetBins)
            {
                pb1.Next($"├─ processing bin {group.Key}: {group.Count()} items");
                var flows = protocolFactory.CollectCoapFlows(group, getModelKeyFunc, getFlowKeyFunc);
                ComputeProfile(profile, flows);
            }
            var pb2 = new ProgressBar(profile.Count);
            profile.Commit(() => pb2.Next($"├─ fitting models"));
            Console.WriteLine($"└ done [{sw.Elapsed}].");
        }

        private static void ComputeProfile(FlowProfile profile, IEnumerable<IFlowRecord> flows) 
        {
            foreach (var flow in flows)
            {
                var modelObject = flow.Model.ToString();
                if (!profile.TryGetValue(modelObject, out var model))
                {
                    model = profile.NewModel();
                    profile[modelObject] = model;
                }
                model.Samples.Add(new double[] { flow.FlowPackets, flow.FlowOctets });
            }
        }
    }
}