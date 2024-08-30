namespace PiecewiseLinearFunction.data
{
    [Serializable]
    public class DictionaryEntry
    {
        public string Key { get; set; }
        public List<InfoBlock> Value { get; set; }

        public DictionaryEntry() { }

        public DictionaryEntry(string key, List<InfoBlock> value)
        {
            Key = key;
            Value = value;
        }
    }
}