using CsvHelper.Configuration.Attributes;
using System;

namespace Ironstone.Analyzers.CoapProfiling
{
    public class CoapPacketRecord : IPacketRecord
    {
        [Flags]
        public enum Fields { Unknown = 0, FrameTimeEpoch = 1, IpSrc = 2, IpDst = 4, UdpSrcPort = 8, UdpDstPort = 16, UdpLength = 32, CoapCode = 64, CoapType = 128, CoapMid = 256, CoapToken = 512, CoapOptUriPathRecon = 1024, CoapUriHost = 2048 }

        [Name("frame.time_epoch")]
        public double TimeEpoch { get; set; }

        [Name("ip.src")]
        public string IpSrc { get; set; }

        [Name("udp.srcport")]
        public int SrcPort { get; set; }

        [Name("ip.dst")]
        public string IpDst { get; set; }

        [Name("udp.dstport")]
        public int DstPort { get; set; }

        [Name("udp.length")]
        public int PayloadLength { get; set; }

        [Name("coap.code")]
        public int CoapCode { get; set; }

        [Name("coap.type")]
        public int CoapType { get; set; }

        [Name("coap.mid")]
        public int CoapMessageId { get; set; }

        [Name("coap.token")]
        public string CoapToken { get; set; }

        [Name("coap.opt.uri_path_recon")]
        public string CoapUriPath { get; set; }

        /// <summary>
        /// Gets Coap Host URI part deduced from Coap Code and flow information.
        /// </summary>
        public string CoapUriHost => CoapCode < 32 ? $"{IpDst}:{DstPort}" : $"{IpSrc}:{SrcPort}";
    }
}

