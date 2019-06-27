using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ironstone.Analyzers.CoapProfiling
{

    [Serializable]
    public abstract class ProtocolFactory
    {
        public abstract string Name { get;}
        public abstract Enum GetModelKeyFields(string modelKeyString);

        public abstract IFlowRecord GetFlow(IGrouping<string, IPacketRecord> arg);


        [Serializable]
        class CoapProtocolFactory : ProtocolFactory
        {
            public override string Name => "CoAP";

            public override IFlowRecord GetFlow(IGrouping<string, IPacketRecord> arg)
            {
                return CoapFlowRecord.GetFlow(arg);
            }

            public override Enum GetModelKeyFields(string modelKeyString)
            {
                if (String.IsNullOrWhiteSpace(modelKeyString)) return CoapResourceAccess.Fields.CoapCode | CoapResourceAccess.Fields.CoapType | CoapResourceAccess.Fields.CoapUriPath;
                return FieldHelper.Parse<CoapResourceAccess.Fields>(modelKeyString, (x, y) => (x | y));
            }

            public override Func<IPacketRecord, string> GetModelKeyFunc(Enum modelKey)
            {
                return x => CoapResourceAccess.GetModelKeyFunc((CoapResourceAccess.Fields)modelKey)((CoapPacketRecord)x);
            }
        }

        [Serializable]
        class IecProtocolFactory : ProtocolFactory
        {
            public override string Name => "IEC";

            public override IFlowRecord GetFlow(IGrouping<string, IPacketRecord> arg)
            {
                throw new NotImplementedException();
            }

            public override Enum GetModelKeyFields(string modelKeyString)
            {
                throw new NotImplementedException();
            }

            public override Func<IPacketRecord, string> GetModelKeyFunc(Enum modelKey)
            {
                throw new NotImplementedException();
            }
        }

        public enum SupportedProtocols { Coap, Iec }

        public static ProtocolFactory Create(string protocol)
        {
            if (Enum.TryParse< SupportedProtocols >(protocol, true, out var protoType))
            {
                switch(protoType)
                {
                    case SupportedProtocols.Coap: return new CoapProtocolFactory();
                    case SupportedProtocols.Iec: return new IecProtocolFactory();
                }
            }
            throw new ArgumentException($"Protocol {protocol} not recognized nor supported.");
        }

        public abstract Func<IPacketRecord, string> GetModelKeyFunc(Enum modelKey);

        public IEnumerable<IFlowRecord> CollectCoapFlows(IEnumerable<IPacketRecord> packets, Func<IPacketRecord, string> getModelKeyFunc, Func<IPacketRecord, string> getFlowKeyFunc)
        {
            return packets.GroupBy(getModelKeyFunc).SelectMany(g => g.GroupBy(getFlowKeyFunc).Select(GetFlow));
        }
    }
}
