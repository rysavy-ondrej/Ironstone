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

        public static FlowProfile Create(Type typ, string[] dimensions, double windowSize)
        {
            if (typ == typeof(CoapStatisticalModel)) return new FlowProfile(dimensions, windowSize, new StatisticalModelFactory());
            if (typ == typeof(CoapMixtureModel)) return new FlowProfile(dimensions, windowSize, new CoapMixtureModelFactory());
            if (typ == typeof(CoapStatisticalFingerprint)) return new FlowProfile(dimensions, windowSize,new CoapStatisticalFingerprintFactory());
            return null;
        }
    }
}
