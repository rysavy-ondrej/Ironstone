using ConsoleTableExt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Ironstone.Analyzers.CoapProfiling
{

    [Serializable]
    public class FlowProfile : ISerializable
    { 
        public string[] Dimensions { get; set; }
        public double WindowSize { get; set; }
        IDictionary<string, IFlowModel> profileDictionary = new Dictionary<string, IFlowModel>();

        public FlowProfile(string[] dimensions, double windowSize, IFlowModelFactory builder) { Dimensions = dimensions; WindowSize = windowSize; ModelBuilder = builder; }

        public ICollection<string> Keys => profileDictionary.Keys;

        public ICollection<IFlowModel> Values => profileDictionary.Values;

        public IList<KeyValuePair<string, IFlowModel>> Items => profileDictionary.ToList();


        public int Count => profileDictionary.Count;

        public double ThresholdMultiplier { get; set; } = 1;
        public IFlowModelFactory ModelBuilder { get; private set; }
            

        public IFlowModel this[string key] { get => profileDictionary[key]; set => profileDictionary[key] = value; }

        public void Add(string key, IFlowModel value)
        {
            profileDictionary.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return profileDictionary.ContainsKey(key);
        }

        public bool TryGetValue(string key, out IFlowModel value)
        {
            return profileDictionary.TryGetValue(key, out value);
        }

        public void Clear()
        {
            profileDictionary.Clear();
        }

        public IEnumerator<KeyValuePair<string, IFlowModel>> GetEnumerator()
        {
            return profileDictionary.GetEnumerator();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Dimensions), Dimensions);
            info.AddValue(nameof(WindowSize), WindowSize);
            info.AddValue(nameof(ModelBuilder), ModelBuilder);
            info.AddValue(nameof(profileDictionary.Count), profileDictionary.Count);
            var i = 0;
            foreach(var profile in profileDictionary)
            {
                info.AddValue($"key_{i}", profile.Key);
                info.AddValue($"val_{i}", profile.Value);
                i++;
            }
        }

        public FlowProfile(SerializationInfo info, StreamingContext context)
        {
            Dimensions = (string[])info.GetValue(nameof(Dimensions), typeof(string[]));
            WindowSize = info.GetDouble(nameof(WindowSize));
            ModelBuilder = (IFlowModelFactory)info.GetValue(nameof(ModelBuilder), typeof(IFlowModelFactory));
            var count = info.GetInt32(nameof(profileDictionary.Count));
            for(int i = 0; i <count; i++)
            {
                var key = info.GetString($"key_{i}");
                var value = (IFlowModel)info.GetValue($"val_{i}", typeof(IFlowModel));
                this.profileDictionary.Add(key, value);
            }
            foreach (var model in profileDictionary) model.Value.Fit();
        }

        public void Dump(TextWriter writer)
        {
            var first = profileDictionary.First();
            var info = first.Value.Info;

            var profileTable = new DataTable();
            profileTable.Columns.Add("Name", typeof(string));
            profileTable.Columns.Add("Observations", typeof(int));
            profileTable.Columns.Add("Threshold", typeof(double));
            foreach(var infoKey in info.Keys)
            {
                profileTable.Columns.Add(infoKey, typeof(string));
            }           
            foreach (var model in this.profileDictionary)
            {
                var row = profileTable.NewRow();
                row[0] = $"{model.Key}";
                row[1] = model.Value.Samples.Count;
                row[2] = model.Value.Threshold;
                var i = 3;
                foreach (var infoKey in info.Keys)
                {
                    row[i++] = model.Value.Info[infoKey];
                }
                profileTable.Rows.Add(row);
            }

            var sb = ConsoleTableBuilder.From(profileTable)
               .WithFormat(ConsoleTableBuilderFormat.MarkDown).Export();
            writer.WriteLine(sb.ToString());
        }

        internal void Commit(Action progressCallback = null)
        {
            foreach (var model in profileDictionary)
            {
                model.Value.Fit();
                progressCallback?.Invoke();
            }
        }



        public IFlowModel NewModel()
        {
            return ModelBuilder.NewModel(this.Dimensions);
        }
    }
}
