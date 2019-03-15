using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using ConsoleTableExt;
using CsvHelper;
using Microsoft.Extensions.CommandLineUtils;
using ShellProgressBar;

namespace Ironstone.Analyzers.CoapProfiling
{



    internal struct LearningWindow
    {
        public enum ValueType { Absolute, Ratio };
        public ValueType Meassure { get; set; }
        public double Value { get; set; }   
        public static LearningWindow Parse(string input)
        {
            var str = input.Trim();
            if (str.EndsWith('%'))
            {
                double.Parse(str.TrimEnd('%'));
                return new LearningWindow
                {
                    Meassure = ValueType.Ratio,
                    Value = double.Parse(str.TrimEnd('%')) / 100.0
                };
            }
            else
            {
                return new LearningWindow
                {
                    Meassure = ValueType.Absolute,
                    Value = double.Parse(str)
                };
            }
        }
        public static LearningWindow All => new LearningWindow { Meassure = ValueType.Ratio, Value = 1.0 };
    }

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

            var readOption = command.Option("-InputFile <filename.csv>",
                "A name of input file in csv format that contains decoded CoAP packets.",
                CommandOptionType.MultipleValue);

            var writeOption = command.Option("-WriteTo <filename>",
                "An output file to write binary representation of the profile.",
                CommandOptionType.SingleValue);

            var windowSizeOption = command.Option("-WindowSize <double>",
                "A size of windows in seconds.",
                CommandOptionType.MultipleValue);

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
                var windowSize = windowSizeOption.HasValue() ? Double.Parse(windowSizeOption.Value()) : DefaultWindowSize;
                var flowAggregation = aggregateOption.HasValue() ? FieldHelper.Parse<FlowKey.Fields>(aggregateOption.Value(), (x,y)=>(x|y)) : FlowKey.Fields.None;
                var modelKey = modelKeyOption.HasValue() ? FieldHelper.Parse<CoapResourceAccess.Fields>(modelKeyOption.Value(), (x, y) => (x | y)) : CoapResourceAccess.Fields.CoapCode | CoapResourceAccess.Fields.CoapType | CoapResourceAccess.Fields.CoapUriPath;
                var modelClassType = modelClassOption.HasValue() ? GetModelType(modelKeyOption.Value()) : typeof(CoapMixtureModel);
                var windowsCount = windowsCountOption.HasValue() ? LearningWindow.Parse(windowsCountOption.Value()) : LearningWindow.All;

                if (readOption.HasValue() && writeOption.HasValue())
                {
                    CreateProgressBar();
                    var profile = ProfileFactory.Create(modelClassType, new string[] { "Packets","Octets" }, windowSize);
                    LoadAndCompute(profile, readOption.Value(), windowSize, windowsCount, modelKey, flowAggregation);
                    StoreProfile(profile, writeOption.Value());
                    return 0;
                }
                throw new CommandParsingException(command, $"Missing required arguments: {readOption.ShortName}, {writeOption.ShortName}.");
            });
        }

        private Type GetModelType(string className)
        {
            return Assembly.GetExecutingAssembly().GetType(className, true, true);
        }

        ProgressBar progressBar; 
        private void CreateProgressBar()
        {
            progressBar = new ProgressBar(0, String.Empty, new ProgressBarOptions
            {
                ProgressCharacter = '-',
            });
        }
        private void ResetProgressBar(int count, string message)
        {
            progressBar.Tick(0);
            progressBar.MaxTicks = count;
            progressBar.Message = message;
        }

        private void StoreProfile(CoapProfile profile, string outputFile)
        {
            var formatter = new BinaryFormatter();
            using (var s = new FileStream(outputFile, FileMode.Create))
            {
                formatter.Serialize(s, profile);
                s.Close();
            }
        }

        private void LoadAndCompute(CoapProfile profile, string inputFile, double windowSize, LearningWindow learningWindows, CoapResourceAccess.Fields modelKey, FlowKey.Fields flowAggregation = FlowKey.Fields.None) 
        {
            var getFlowKeyFunc = FlowKey.GetFlowKeyFunc(flowAggregation);
            var getModelKeyFunc = CoapResourceAccess.GetModelKeyFunc(modelKey);
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

                var learningBins = learningWindows.Meassure == LearningWindow.ValueType.Absolute ? packetBins.Take((int)learningWindows.Value) :
                    packetBins.Take((int)(packetBins.Count() * learningWindows.Value));

                ResetProgressBar(packetBins.Count(), $"Processing bins...");
                foreach (var group in packetBins)
                {
                    progressBar.Tick();
                    var flows = CoapFlowRecord.CollectCoapFlows(group, getModelKeyFunc, getFlowKeyFunc);
                    ComputeProfile(profile, flows);
                }
            }
            ResetProgressBar(profile.Count, $"Fitting models...");
            profile.Commit(() => progressBar.Tick());
        }

        private static void ComputeProfile(CoapProfile profile, IEnumerable<CoapFlowRecord> flows) 
        {
            foreach (var flow in flows)
            {
                var coapObject = flow.CoapObject.ToString();
                if (!profile.TryGetValue(coapObject, out var model))
                {
                    model = profile.NewModel();
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