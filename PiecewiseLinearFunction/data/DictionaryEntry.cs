namespace PiecewiseLinearFunction.data
{
    [Serializable]
    public class DictionaryEntry
    {
        public string Key { get; set; }
        public List<Vertex> Value { get; set; }

        public DictionaryEntry() { }

        public DictionaryEntry(string key, List<Vertex> value)
        {
            Key = key;
            Value = value;
        }
    }
}