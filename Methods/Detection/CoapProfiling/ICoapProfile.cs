using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Ironstone.Analyzers.CoapProfiling
{
    public interface ICoapModel : ISerializable
    {
        List<double[]> Samples { get; }
        double Threshold { get; }
        void Fit();
        double Score(double[] sample);
        Dictionary<string,string> Info { get; }
    }
}