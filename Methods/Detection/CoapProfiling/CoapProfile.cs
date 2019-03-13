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

    public class ProfileFactory
    {
        public static CoapProfile Create(Type typ, string[] dimensions, double windowSize)
        {
            if (typ == typeof(CoapStatisticalModel)) return new CoapProfile(dimensions, windowSize, d => new CoapStatisticalModel(d));
            if (typ == typeof(CoapMixtureModel)) return new CoapProfile(dimensions, windowSize, d => new CoapMixtureModel(d));
            if (typ == typeof(CoapStatisticalFingerprint)) return new CoapProfile(dimensions, windowSize, d => new CoapStatisticalFingerprint(d));
            return null;
        }
    }

    [Serializable]
    public class CoapProfile : ISerializable
    { 
        public string[] Dimensions { get; set; }
        public double WindowSize { get; set; }

        private readonly Func<string[], ICoapModel> m_modelBuilder;
        IDictionary<string, ICoapModel> profileDictionary = new Dictionary<string, ICoapModel>();

        public CoapProfile(string[] dimensions, double windowSize, Func<string[], ICoapModel> builder) { Dimensions = dimensions; WindowSize = windowSize; m_modelBuilder = builder; }

        public ICollection<string> Keys => profileDictionary.Keys;

        public ICollection<ICoapModel> Values => profileDictionary.Values;

        public int Count => profileDictionary.Count;

        public ICoapModel this[string key] { get => profileDictionary[key]; set => profileDictionary[key] = value; }

        public void Add(string key, ICoapModel value)
        {
            profileDictionary.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return profileDictionary.ContainsKey(key);
        }

        public bool TryGetValue(string key, out ICoapModel value)
        {
            return profileDictionary.TryGetValue(key, out value);
        }

        public void Clear()
        {
            profileDictionary.Clear();
        }

        public IEnumerator<KeyValuePair<string, ICoapModel>> GetEnumerator()
        {
            return profileDictionary.GetEnumerator();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("winsize", WindowSize);
            info.AddValue("count", profileDictionary.Count);
            var i = 0;
            foreach(var profile in profileDictionary)
            {
                info.AddValue($"key_{i}", profile.Key);
                info.AddValue($"val_{i}", profile.Value);
                i++;
            }
        }

        public CoapProfile(SerializationInfo info, StreamingContext context)
        {
            WindowSize = info.GetDouble("winsize");
            var count = info.GetInt32("count");
            for(int i = 0; i <count; i++)
            {
                var key = info.GetString($"key_{i}");
                var value = (ICoapModel)info.GetValue($"val_{i}", typeof(ICoapModel));
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



        public ICoapModel NewModel()
        {
            return m_modelBuilder(this.Dimensions);
        }
    }
}
