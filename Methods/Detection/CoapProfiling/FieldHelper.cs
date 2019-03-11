using System;
using System.Text.RegularExpressions;

namespace Ironstone.Analyzers.CoapProfiling
{                                               
    public static class FieldHelper
    {
        public static string Normalize(string input)
        {                            
            return Regex.Replace(input, @"[_\.]", "");
        }
        public static T Parse<T>(string fieldNames, Func<T,T,T> combine) where T : struct 
        {
            if (String.IsNullOrWhiteSpace(fieldNames)) return default;
            var items = fieldNames.Split(',');
            T allFlags = default;
            foreach (var item in items)
            {
                var val = Enum.TryParse<T>(FieldHelper.Normalize(item), true, out var result) ? result : default;
                allFlags = combine(allFlags, val);
            }
            return allFlags;
        }
    }
}

