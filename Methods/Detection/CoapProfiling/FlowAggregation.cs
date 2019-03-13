using System;
using System.Text.RegularExpressions;

namespace Ironstone.Analyzers.CoapProfiling
{
    public class FlowKey
    {
        [Flags]
        public enum Fields { None = 0, IpSrc = 1, IpDst = 2, UdpSrcPort = 4, UdpDstPort = 8 }
        public string IpSrc { get; internal set; }
        public int SrcPort { get; internal set; }
        public string IpDst { get; internal set; }
        public int DstPort { get; internal set; }

        public static Func<CoapPacketRecord, string> GetFlowKeyFunc(FlowKey.Fields aggregation = Fields.None)
        {
            return p =>
            {
                var fk = new FlowKey
                {
                    IpSrc = aggregation.HasFlag(FlowKey.Fields.IpSrc) ? "0.0.0.0" : p.IpSrc,
                    SrcPort = aggregation.HasFlag(FlowKey.Fields.UdpSrcPort) ? 0 : p.SrcPort,
                    IpDst = aggregation.HasFlag(FlowKey.Fields.IpDst) ? "0.0.0.0" : p.IpDst,
                    DstPort = aggregation.HasFlag(FlowKey.Fields.UdpDstPort) ? 0 : p.DstPort
                };
                return fk.ToString();
            };
        }

        public override string ToString()
        {
            return $"{IpSrc}.{SrcPort}>{IpDst}.{DstPort}";

        }
        public static FlowKey Parse(string flowKey)
        {
            var m = Regex.Match(flowKey, @"(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})\.(\d{1,5})>(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})\.(\d{1,5})");
            if (m.Success)
            {

                return new FlowKey
                {
                    IpSrc = m.Groups[1].Value,
                    SrcPort = Int32.Parse(m.Groups[2].Value),
                    IpDst = m.Groups[3].Value,
                    DstPort = Int32.Parse(m.Groups[4].Value)
                };
            }
            else
                return new FlowKey(); 
        
        }
    }
}