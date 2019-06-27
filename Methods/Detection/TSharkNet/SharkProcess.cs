using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Ironstone.Analyzers.TSharkNet
{
    public class SharkProcess
    {
        public static string SharkPath { get; set; }

        public IEnumerable<JObject> RunForJson(string inputfile, string filter)
        {
            var arguments = $"-r {inputfile} -Y {filter} -T ek";
            foreach (var line in Run(arguments))
            {
                if (line.StartsWith("{\"timestamp\""))
                {
                    yield return JObject.Parse(line);
                }
            }
        }

        public IEnumerable<string> RunForFields(string inputfile, string filter, string separator, params string[] fields)
        {
            var fieldString = String.Join(" -e ", fields);
            var arguments = $"-r {inputfile} -Y \"{filter}\" -T fields -E separator={separator} -e {fieldString}";
            foreach(var line in Run(arguments))
            {
                yield return line;
            }
        }

        private IEnumerable<string> Run(string arguments)
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = SharkPath ?? "tshark.exe",
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            while (true)
            {
                string line = process.StandardOutput.ReadLine();
                if (line != null)
                {
                    yield return line;
                }
                else
                {
                    break;
                }
            }

            process.WaitForExit();
        }
    }
}
