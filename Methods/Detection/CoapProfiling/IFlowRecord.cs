namespace Ironstone.Analyzers.CoapProfiling
{
    public interface IFlowRecord
    {
        string DstAddr { get; set; }
        int DstPort { get; set; }
        double EndMsec { get; set; }
        int FlowOctets { get; set; }
        int FlowPackets { get; set; }
        FlowKey Key { get; }
        object  Model { get; }
        string SrcAddr { get; set; }
        int SrcPort { get; set; }
        double StartMsec { get; set; }
        
    }
}