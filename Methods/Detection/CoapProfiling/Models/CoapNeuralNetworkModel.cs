using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Ironstone.Analyzers.CoapProfiling.Models
{
    class CoapNeuralNetworkModel : IFlowModel
    {
        private string[] m_dimensions;

        public CoapNeuralNetworkModel(string[] dimensions)
        {
            m_dimensions = dimensions;
        }

        public List<double[]> Samples => throw new NotImplementedException();

        public double Threshold => throw new NotImplementedException();

        public Dictionary<string, string> Info => throw new NotImplementedException();

        public void Fit()
        {
            throw new NotImplementedException();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        public double Score(double[] sample)
        {
            throw new NotImplementedException();
        }
    }
}
