namespace Ironstone.Analyzers.CoapProfiling
{
    public interface IPacketRecord
    {
        int DstPort { get; }
        string IpDst { get; }
        string IpSrc { get; }
        int SrcPort { get; }
        double TimeEpoch { get; }
        int PayloadLength { get; }
    }
}