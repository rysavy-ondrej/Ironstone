using Microsoft.Extensions.CommandLineUtils;

namespace Ironstone.Analyzers.CoapProfiling
{
    class Program
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            logger.Debug("Application started.");

            var commandLineApplication = new CommandLineApplication
            {
                Name = "Ironstone.Analyzers.CoapProfiling"
            };
            commandLineApplication.HelpOption("-?|-Help");

            commandLineApplication.Command("Learn-Profile", configuration: new LearnProfile().Configuration);
            commandLineApplication.Command("Test-Capture", configuration: new TestCapture().Configuration);
            commandLineApplication.Command("Print-Profile", configuration: new PrintProfile().Configuration);
            commandLineApplication.Command("Compute-Distance", configuration: new ComputeDistance().Configuration);

            commandLineApplication.OnExecute(() =>
            {
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

        }
    }
}

