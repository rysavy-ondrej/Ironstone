using AutoMapper;
using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ironstone.Analyzers.CoapProfiling
{
    public class CoapFlowRecord
    {
        [Name("cflow.abstimestart")]
        public double StartMsec { get; set; }
        [Name("cflow.abstimeend")]
        public double EndMsec { get; set; }

        [Name("cflow.srcaddr")]
        public string SrcAddr { get; set; }

        [Name("cflow.srcport")]
        public int SrcPort { get; set; }

        [Name("cflow.dstaddr")]
        public string DstAddr { get; set; }

        [Name("cflow.dstport")]
        public int DstPort { get; set; }

        [Name("cflow.packets")]
        public int FlowPackets { get; set; }

        [Name("cflow.octets")]
        public int FlowOctets { get; set; }

        [Name("flow.coap.code")]
        public int CoapCode { get; set; }

        [Name("flow.coap.type")]
        public int CoapType { get; set; }

        [Name("flow.coap.uri_path")]
        public string CoapUriPath { get; set; }

        public string FlowKey => $"{SrcAddr}.{SrcPort}->{DstAddr}.{DstPort}";

        public CoapResourceAccess CoapObject => new CoapResourceAccess(CoapCode, CoapType, CoapUriPath);

        public static IEnumerable<CoapFlowRecord> CollectFlows(IEnumerable<CoapPacketRecord> packets, Func<CoapPacketRecord,string> getKey)
        {
            return packets.GroupBy(getKey).Select(p =>
            {
                var first = p.First();
                var last = p.Last();
                return new CoapFlowRecord
                {
                    StartMsec = first.TimeEpoch,
                    EndMsec = last.TimeEpoch,
                    SrcAddr = first.IpSrc,
                    SrcPort = first.SrcPort,
                    DstAddr = first.IpDst,
                    DstPort = first.DstPort,
                    CoapCode = first.CoapCode,
                    CoapType = first.CoapType,
                    CoapUriPath = first.CoapUriPath,
                    FlowOctets = p.Sum(x => x.UdpLength),
                    FlowPackets = p.Count()
                };
            });
        }
    }
}