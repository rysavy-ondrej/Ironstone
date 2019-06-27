namespace Ironstone.Analyzers.CoapProfiling
{
    public class IecAsduPacket : IPacketRecord
    {
        public double TimeEpoch { get; set; }
        public string IpSrc { get; set; }
        public string IpDst { get; set; }

        public int DstPort { get; set; }
        public int SrcPort { get; set; }
        public int PayloadLength { get; set; }

        public int Causetx { get; set; }
        public string Cot
        {
            get
            {
                switch (Causetx)
                {

                    case 1: return "per/cyc(1)";
                    case 2: return "back(2)";
                    case 3: return "spont(3)";
                    case 4: return "init(4)";
                    case 5: return "req(5)";
                    case 6: return "act(6)";
                    case 7: return "actcon(7)";
                    case 8: return "deact(8)";
                    case 9: return "deactcon(9)";
                    case 10: return "actterm(10)";
                    case 11: return "retrem(11)";
                    case 12: return "retloc(12)";
                    case 13: return "file(13)";
                    default: return $"val({Causetx})";
                }
            }
        }
    }
}