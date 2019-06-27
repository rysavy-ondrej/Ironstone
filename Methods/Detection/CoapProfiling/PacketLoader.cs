using CsvHelper;
using Ironstone.Analyzers.TSharkNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static Ironstone.Analyzers.CoapProfiling.CreateTimeline;

namespace Ironstone.Analyzers.CoapProfiling
{
    public static class PacketLoader
    {
        public static IEnumerable<IecAsduPacket> LoadIecPacketsFromCap(string inputFile)
        {
            var sharkProcess = new SharkProcess();
            var items = sharkProcess.RunForFields(inputFile, "104asdu", "|", "frame.time_epoch", "ip.src", "ip.dst", "104asdu.causetx");
            foreach (var item in items)
            {
                var fields = item.Split('|');
                var ctxlist = fields[3].Split(',');
                foreach (var ctx in ctxlist)
                {
                    yield return new IecAsduPacket
                    {
                        TimeEpoch = Double.Parse(fields[0]),
                        IpSrc = fields[1],
                        IpDst = fields[2],
                        Causetx = Int32.Parse(ctx)
                    };
                }
            }
        }

        public static IEnumerable<CoapPacketRecord> LoadCoapPacketsFromCsv(string inputFile)
        {

            using (var reader = new StreamReader(inputFile))
            using (var csv = new CsvReader(reader))
            {
                csv.Configuration.PrepareHeaderForMatch = (string header, int index) => header.Trim();
                csv.Configuration.MissingFieldFound = null;
                csv.Configuration.HeaderValidated = null;
                var packets = csv.GetRecords<CoapPacketRecord>().ToList();
                return packets;
            }
        }

        // tshark -T fields -e frame.time_epoch -e ip.src -e ip.dst -e udp.srcport -e udp.dstport -e udp.length -e coap.code -e coap.type -e coap.mid -e coap.token -e coap.opt.uri_path_recon -E header=y -E separator=, -Y "coap && !icmp" -r <INPUT>  > <OUTPUT-CSV-FILE>
        public static IEnumerable<CoapPacketRecord> LoadCoapPacketsFromCap(string inputFile)
        {
            var sharkProcess = new SharkProcess();
            var items = sharkProcess.RunForFields(inputFile, "coap && !icmp", ",", "frame.time_epoch", "ip.src", "ip.dst", "udp.srcport", "udp.dstport", "udp.length", "coap.code", "coap.type", "coap.mid", "coap.token", "coap.opt.uri_path_recon");
            foreach (var item in items)
            {
                var fields = item.Split(',');

                yield return new CoapPacketRecord
                {
                    TimeEpoch = Double.Parse(fields[0]),
                    IpSrc = fields[1],
                    IpDst = fields[2],
                    SrcPort = Int32.Parse(fields[3]),
                    DstPort = Int32.Parse(fields[4]),
                    UdpLength = Int32.Parse(fields[5]),
                    CoapCode = Int32.Parse(fields[6]),
                    CoapType = Int32.Parse(fields[7]),
                    CoapMessageId = Int32.Parse(fields[8]),
                    CoapToken = fields[9],
                    CoapUriPath = fields[10]
                };
            }
        }
    }
}
