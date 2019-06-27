using ConsoleTableExt;
using Ironstone.Analyzers.TSharkNet;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Ironstone.Analyzers.CoapProfiling
{
    public class CreateTimeline
    {
        public CreateTimeline()
        {
        }

        public void Configuration(CommandLineApplication command)
        {
            command.Description = "Create a timeline for CoAP communication from the specific capture file.";
            command.HelpOption("-?|-Help");

            var inputCapOption = command.Option("-InputCapFile <string>",
                "A name of an input file in cap format that contains packets.",
                CommandOptionType.MultipleValue);

            var aggregateOption = command.Option("-Selector <string>",
                "Selector field used to calculate the timeline. It can be any combination of valid fields extracted from the communication. The possible values are: ip.src, ip.dst, udp.srcport, udp.dstport, udp.length, coap.code, coap.type, coap.mid, coap.token, coap.opt.uri_path_recon",
                CommandOptionType.SingleValue);


            var intervalOption = command.Option("-Interval <int>",
                "Specifies the time interval in seconds used for computing timeline.",
                CommandOptionType.SingleValue);

            var protocolOption = command.Option("-Protocol <string>",
                "Specifies the protocol to analyze. Supported protocolas are: coap, iec. Default is coap.",
                CommandOptionType.SingleValue);

            command.OnExecute(() =>
            {
                var interval = intervalOption.HasValue() ? Int32.Parse(intervalOption.Value()) : 1;
                var protocol = protocolOption.Value()?.ToLowerInvariant() ?? "coap";
                if (inputCapOption.HasValue())
                {
                    var inputfile = inputCapOption.Value();
                    var selector = "";
                    switch (protocol)
                    {
                        case "coap":
                            CreateTimelineForCoap(inputfile, interval, selector);
                            break;
                        case "iec":
                            CreateTimelineForIec(inputfile, interval, selector);
                            break;
                    }

                }
                return 0;
            });


        }

        public class IecAsduPacket
        {
            public double TimeEpoch { get; set; }
            public string IpSrc { get; set; }
            public string IpDst { get; set; }
            public int Causetx { get; set; }
            public string Cot
            {
                get
                {
                    switch (Causetx)
                    {

                        case 1:     return "per/cyc(1)";
                        case 2:     return "back(2)";
                        case 3:     return "spont(3)";
                        case 4:     return "init(4)";
                        case 5:     return "req(5)";
                        case 6:     return "act(6)";
                        case 7:     return "actcon(7)";
                        case 8:     return "deact(8)";
                        case 9:     return "deactcon(9)";
                        case 10:    return "actterm(10)";
                        case 11:    return "retrem(11)";
                        case 12:    return "retloc(12)";
                        case 13:    return "file(13)";
                        default:    return $"val({Causetx})";
                    }
                }
            }
        }



        private void CreateTimelineForIec(string inputfile, int interval, string selector)
        {
            var packets = PacketLoader.LoadIecPacketsFromCap(inputfile).ToList();
            CreateTimelineForPackets<IecAsduPacket>(packets, interval, x => x.Cot, x => x.TimeEpoch);
        }

        private void CreateTimelineForCoap(string inputfile, int interval, string selector)
        {
            var packets = PacketLoader.LoadCoapPacketsFromCap(inputfile).ToList();
            CreateTimelineForPackets<CoapPacketRecord>(packets, interval, x => x.CoapUriPath, x=>x.TimeEpoch);
        }

        private void CreateTimelineForPackets<T>(IList<T> packets, int interval, Func<T,string> selector, Func<T,double> timestamp)
        { 
            var paths = packets.Select(selector).ToHashSet();

            var startTime = timestamp(packets.First());
            var points = packets.GroupBy(p => (int)Math.Floor((timestamp(p) - startTime) / interval));
            var timeTable = new DataTable();
            timeTable.Columns.Add("Time", typeof(string));
            foreach (var path in paths)
            {
                var col = new DataColumn(String.IsNullOrWhiteSpace(path) ? "<empty>" : path, typeof(int));
                col.DefaultValue = 0;
                timeTable.Columns.Add(col);
            }
            var lastPoint = 0;
            foreach (var point in points)
            {
                for(int i=lastPoint+1; i<point.Key; i++)
                {   // add empty rows
                    timeTable.Rows.Add(i);
                }
                lastPoint = point.Key;

                var row = timeTable.NewRow();
                var data = point.GroupBy(selector).Select(x => (Key: x.Key, Count: x.Count()));
                row["Time"] = point.Key;
                foreach (var item in data)
                {

                    var key = String.IsNullOrWhiteSpace(item.Key) ? "<empty>" : item.Key;
                    row[key] = item.Count;
                }
                timeTable.Rows.Add(row);
            }

            Console.WriteLine("Timeline:");
            ConsoleTableBuilder.From(timeTable)
                .WithFormat(ConsoleTableBuilderFormat.MarkDown)
                .ExportAndWriteLine();
        }
    }
}