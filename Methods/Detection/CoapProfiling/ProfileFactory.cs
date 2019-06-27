using Ironstone.Analyzers.CoapProfiling.Models;
using System;

namespace Ironstone.Analyzers.CoapProfiling
{
    public class ProfileFactory
    {
        [Serializable]
        class StatisticalModelFactory : IFlowModelFactory
        {
            public IFlowModel NewModel(string[] dimensions) => new CoapStatisticalModel(dimensions);
        }

        [Serializable]
        class CoapStatisticalFingerprintFactory : IFlowModelFactory
        {
            public IFlowModel NewModel(string[] dimensions) => new CoapStatisticalFingerprint(dimensions);
        }

        [Serializable]
        class CoapMixtureModelFactory : IFlowModelFactory
        {
            public IFlowModel NewModel(string[] dimensions) => new CoapMixtureModel(dimensions);
        }
        [Serializable]
        class CoapNeuralNetworkModelFactory : IFlowModelFactory
        {
            public IFlowModel NewModel(string[] dimensions) => new CoapNeuralNetworkModel(dimensions);
        }

        public static FlowProfile Create(ProtocolFactory protocolFactory, FlowKey.Fields flowAggregation, Enum modelKey, Type typ, string[] dimensions, double windowSize)
        {
            if (typ == typeof(CoapStatisticalModel)) return new FlowProfile(protocolFactory,dimensions, windowSize, flowAggregation, modelKey, new StatisticalModelFactory());
            if (typ == typeof(CoapMixtureModel)) return new FlowProfile(protocolFactory,dimensions, windowSize, flowAggregation, modelKey, new CoapMixtureModelFactory());
            if (typ == typeof(CoapStatisticalFingerprint)) return new FlowProfile(protocolFactory,dimensions, windowSize, flowAggregation, modelKey, new CoapStatisticalFingerprintFactory());
            return null;
        }
    }
}
