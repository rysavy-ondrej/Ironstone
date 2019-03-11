using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ironstone.Analyzers.CoapProfiling
{
    public class CoapFlowRecord
    {
        [Flags]
        public enum Fields { Unknown=0, CFlowAbsTimeStart=1, CFlowAbsTimeEnd=2, CFlowSrcAddr=4, CFlowSrcPort=8, CFlowDstAddr=16, CFlowDstPort=32, CFlowPackets=64,
            CFlowOctets=128, FlowCoapCode=256, FlowCoapType=512, FlowCoapUri_Path=1024 } 

        public Fields ParseFieldName(string fieldName) =>
            Enum.TryParse<Fields>(fieldName.Replace(".", ""), true, out var result) ? result : Fields.Unknown;


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

        public CoapResourceAccess CoapObject => new CoapResourceAccess(CoapCode, CoapType, CoapUriPath);

        public FlowKey Key => new FlowKey
        {
            IpSrc = this.SrcAddr,
            SrcPort = this.SrcPort,
            IpDst = this.DstAddr,
            DstPort = this.DstPort
        };

        /// <summary>
        /// Collects CoAP packets into flows. It first groups packets according to the specified model key computed by <paramref name="getModelKey"/>. Then flows are computed within each model group by <paramref name="getFlowKey"/> function. 
        /// </summary>
        /// <param name="packets"></param>
        /// <param name="getModelKey"></param>
        /// <param name="getFlowKey"></param>
        /// <returns></returns>
        public static IEnumerable<CoapFlowRecord> CollectCoapFlows(IEnumerable<CoapPacketRecord> packets, Func<CoapPacketRecord,string> getModelKey, Func<CoapPacketRecord, string> getFlowKey)
        {
            return packets.GroupBy(getModelKey).SelectMany(g => g.GroupBy(getFlowKey).Select(GetFlow));
        }

        private static CoapFlowRecord GetFlow(IGrouping<string, CoapPacketRecord> arg)
        {
            var first = arg.First();
            var last = arg.Last();
            var flowKey =FlowKey.Parse(arg.Key);
            return new CoapFlowRecord
            {
                StartMsec = first.TimeEpoch,
                EndMsec = last.TimeEpoch,
                SrcAddr = flowKey.IpSrc,
                SrcPort = flowKey.SrcPort,
                DstAddr = flowKey.IpDst,
                DstPort = flowKey.DstPort,
                CoapCode = first.CoapCode,
                CoapType = first.CoapType,
                CoapUriPath = first.CoapUriPath,
                FlowOctets = arg.Sum(x => x.UdpLength),
                FlowPackets = arg.Count()
            };
        }
    }
}