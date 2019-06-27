using System;

namespace Ironstone.Analyzers.CoapProfiling
{
    public class CoapResourceAccess 
    {
        [Flags]
        public enum Fields { Unknown = 0, CoapCode = 1, CoapType = 2, CoapUriPath = 4 }

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

        public static Func<CoapPacketRecord, string> GetModelKeyFunc(Fields modelKey)
        {
            return p =>
            {
                var cra = new CoapResourceAccess(p.CoapCode, p.CoapType, p.CoapUriPath);
                return  $"{(modelKey.HasFlag(Fields.CoapCode) ? "*" : cra.CoapCodeString)}"
                      + $"[{(modelKey.HasFlag(Fields.CoapType) ? "*" : cra.CoapTypeString)}]"
                      + $"{(modelKey.HasFlag(Fields.CoapUriPath) ? "*" : cra.CoapUriPath)}";
            };
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
}

