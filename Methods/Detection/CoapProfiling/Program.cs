using Accord.Statistics.Distributions.DensityKernels;
using Accord.Statistics.Testing;
using ConsoleTableExt;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Ironstone.Analyzers.CoapProfiling
{
    class Program
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            logger.Debug("Application started.");

            var commandLineApplication = new CommandLineApplication();
            commandLineApplication.Name = "Ironstone.Analyzers.CoapProfiling";
            commandLineApplication.HelpOption("-?|-Help");

            commandLineApplication.Command("Learn-Profile", configuration: new LearnProfile().Configuration);
            commandLineApplication.Command("Test-Capture", configuration: new TestCapture().Configuration);
            commandLineApplication.Command("Print-Profile", configuration: new PrintProfile().Configuration);

            commandLineApplication.OnExecute(() => {
                commandLineApplication.Error.WriteLine("Error: Command not specified!");
                commandLineApplication.ShowHelp();
                return 0;
            });

            try
            {
                commandLineApplication.Execute(args);
            }
            catch (CommandParsingException e)
            {
                commandLineApplication.Error.WriteLine($"Error: {e.Message}");
                commandLineApplication.ShowHelp();
            }
            logger.Debug("Application ended.");
             /*
 

                var truePositive = 0;  // true positive is an outcome where the model correctly predicts the positive class.
                var trueNegative = 0;  // true negative is an outcome where the model correctly predicts the negative class
                var falsePositive = 0; // false positive is an outcome where the model incorrectly predicts the positive class.
                var falseNegative = 0; // false negative is an outcome where the model incorrectly predicts the negative class

                // Test against the whole collection
                Console.WriteLine("SOURCE SAMPLES");
                foreach (var window in items.GroupBy(x => (int)Math.Floor((x.StartMsec - start) / windowSize)))
                {
                    var (pos, neg) = (1, 1);// TestFlows(null, null);
                    truePositive += pos;
                    falseNegative += neg;
                }
             
                var items2 = RandomData(profiles, start, stop, items.Count()).ToList();
                Console.WriteLine("RANDOM SAMPLES");
                // Test against the randomly generated items
                foreach (var window in items2.GroupBy(x => (int)Math.Floor((x.Start - start) / windowSize)))
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
                Console.WriteLine("Confusion Matrix:");
                ConsoleTableBuilder.From(confusionMatrix)
                   .WithFormat(ConsoleTableBuilderFormat.MarkDown)
                   .ExportAndWriteLine();
                   */
            }
        }
       
    }

