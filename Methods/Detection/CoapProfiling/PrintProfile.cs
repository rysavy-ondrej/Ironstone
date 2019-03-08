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

            command.OnExecute(() =>
            {
                if (readOption.HasValue())
                {
                    var profile = LoadProfile(readOption.Value());
                    profile.Dump(Console.Out);
                    return 0;
                }
                throw new CommandParsingException(command, $"Missing required argument {readOption.ShortName}.");
            });
        }

        internal static CoapProfile<CoapStatisticalModel> LoadProfile(string fileName)
        {
            IFormatter formatter = new BinaryFormatter();
            var s = new FileStream(fileName, FileMode.Open);
            var pm = (CoapProfile<CoapStatisticalModel>)formatter.Deserialize(s);
            return pm;
        }

        

    }
}
