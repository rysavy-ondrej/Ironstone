using CsvHelper.Configuration.Attributes;

namespace Ironstone.Analyzers.CoapProfiling
{

    public class CoapResourceAccess
    {
        public int CoapCode { get; set; }
        public int CoapType { get; set; }
        public string CoapUriPath { get; set; }


        public CoapResourceAccess(int code, int type, string path)
        {
            CoapCode = code;
            CoapType = type;
            CoapUriPath = path;
        }
        public override string ToString()
        {
            return $"{CoapCodeString}[{CoapTypeString}]:{CoapUriPath}";
        }

        public string CoapCodeString
        {
            get
            {
                switch (CoapCode)
                {
                    case 0: return "0.00(Empty)";
                    case 1: return "0.01(Get)";
                    case 2: return "0.02(Post)";
                    case 3: return "0.03(Put)";
                    case 4: return "0.04(Delete)";
                    case 65: return "2.01(Created)";
                    case 66: return "2.02(Deleted)";
                    case 68: return "2.04(Changed)";
                    case 69: return "2.05(Content)";
                    case 100: return "4.04(Not found)";
                    case 101: return "4.05(Method not allowed)";
                    default:
                        return $"{(CoapCode >> 5)}.{(CoapCode & 0x1f):00}";
                }
            }
        }
        public string CoapTypeString
        {
            get
            {
                switch (CoapType)
                {
                    case 0: return "CON";
                    case 1: return "NON";
                    case 2: return "ACK";
                    case 3: return "RST";
                    default: return CoapType.ToString();
                }
            }
        }

    }
    public class CoapPacketRecord
    {
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
        public int UdpLength { get; set; }

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

        public CoapResourceAccess CoapObject => new CoapResourceAccess(CoapCode, CoapType, CoapUriPath);
    }
}
