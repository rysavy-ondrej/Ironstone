namespace Ironstone.Analyzers.CoapProfiling
{
    internal struct LearningWindow
    {
        public enum ValueType { Absolute, Ratio };
        public ValueType Meassure { get; set; }
        public double Value { get; set; }   
        public static LearningWindow Parse(string input)
        {
            var str = input.Trim();
            if (str.EndsWith('%'))
            {
                double.Parse(str.TrimEnd('%'));
                return new LearningWindow
                {
                    Meassure = ValueType.Ratio,
                    Value = double.Parse(str.TrimEnd('%')) / 100.0
                };
            }
            else
            {
                return new LearningWindow
                {
                    Meassure = ValueType.Absolute,
                    Value = double.Parse(str)
                };
            }
        }
        public static LearningWindow All => new LearningWindow { Meassure = ValueType.Ratio, Value = 1.0 };
    }
}