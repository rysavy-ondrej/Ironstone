using Accord.Math.Distances;
using ConsoleTableExt;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Ironstone.Analyzers.CoapProfiling
{
    class ComputeDistance
    {
        public void Configuration(CommandLineApplication command)
        {
            command.Description = "Computes the distance between historogrmas of two CoAP profiles using Bhattacharyya method.";
            command.HelpOption("-?|-Help");

            var profileOption = command.Option("-ProfileFile <string>",
                "A name of a profile file.",
                CommandOptionType.MultipleValue);

            command.OnExecute(() =>
            {
                var profiles = profileOption.Values.Select(PrintProfile.LoadProfile).ToList();
                if (profiles.Count != 2) throw new ArgumentException("Exactly two profiles should be specified.");
                ComputeDistances(profiles[0], profiles[1]);
                return 0;
            });
        }


        private void ComputeDistances(FlowProfile profile1, FlowProfile profile2)
        {
            var measuresTable = new DataTable();
            measuresTable.Columns.Add("Target", typeof(string));
            measuresTable.Columns.Add("Count 1", typeof(double));
            measuresTable.Columns.Add("Count 2", typeof(double));
            measuresTable.Columns.Add("Distance", typeof(double));
            foreach (var m1 in profile1)
            {
                if (profile2.TryGetValue(m1.Key, out var m2))
                {
                    var b = new Bhattacharyya();
                    var dist = b.Distance(m1.Value.Samples.ToArray(), m2.Samples.ToArray());
                    measuresTable.Rows.Add(m1.Key, m1.Value.Samples.Count, m2.Samples.Count, dist);
                }
            }

            Console.WriteLine("Distances:");
            ConsoleTableBuilder.From(measuresTable)
                .WithFormat(ConsoleTableBuilderFormat.MarkDown)
                .ExportAndWriteLine();

        }
    }
}
