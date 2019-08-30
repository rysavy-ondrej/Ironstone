using ConsoleTableExt;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Ironstone.Analyzers.CoapProfiling
{
    class PrintProfile
    {
        private SafeFileHandle fileName;

        public void Configuration(CommandLineApplication command)
        {
            command.Description = "Prints the CoAP profiles read the specific capture file.";
            command.HelpOption("-?|-Help");

            var readOption = command.Option("-InputFile <filename.profile>",
                        "A name of input file containing the serialized profiles.",
                        CommandOptionType.MultipleValue);
            var printPdf = command.Option("-PrintPdf", "Prints PDF of all models as CSV suitable for visualization.", CommandOptionType.NoValue);

            command.OnExecute(() =>
            {
                if (readOption.HasValue())
                {
                    var profile = LoadProfile(readOption.Value());
                    profile.Dump(Console.Out);
                    if (printPdf.HasValue())
                    { 
                        DumpPdf(profile);
                    }
                    return 0;
                }
                throw new CommandParsingException(command, $"Missing required argument {readOption.ShortName}.");
            });
        }

        /// <summary>
        /// Prints PDF of all models as CSV suitable for visualization. 
        /// </summary>
        /// <param name="profile"></param>
        private void DumpPdf(FlowProfile profile)
        {
            foreach(var model in profile)
            {
                Console.WriteLine($"{model.Key}:");
                Console.WriteLine($"x,y,score");
                var samples = model.Value.Samples;
                var xmin = samples.Select(x => x[0]).Min(); var xmax = samples.Select(x => x[0]).Max();
                var ymin = samples.Select(x => x[1]).Min(); var ymax = samples.Select(x => x[1]).Max();
                var xdelta = (xmax - xmin) / 10;
                var ydelta = (ymax - ymin) / 10;

                for (var x = xmin; x < xmax; x+= xdelta)
                    for(var y = ymin; y < ymax; y +=ydelta)
                    {
                        Console.WriteLine($"{x},{y},{model.Value.Score(new[] { x,y})}");
                    }
                Console.WriteLine();
            }
        }

        internal static FlowProfile LoadProfile(string fileName) 
        {
            IFormatter formatter = new BinaryFormatter();
            var s = new FileStream(fileName, FileMode.Open);
            var pm = (FlowProfile)formatter.Deserialize(s);
            return pm;
        }

        

    }
}
