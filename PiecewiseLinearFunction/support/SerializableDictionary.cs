using PiecewiseLinearFunction.data;

namespace PiecewiseLinearFunction.support
{
    [Serializable]
    public class SerializableDictionary
    {
        public List<DictionaryEntry> Entries { get; set; } = new List<DictionaryEntry>();

        public SerializableDictionary()
        {
        }

        public SerializableDictionary(Dictionary<string, List<Vertex>> dictionary)
        {
            foreach (var kvp in dictionary)
            {
                Entries.Add(new DictionaryEntry(kvp.Key, kvp.Value));
            }
        }

        public Dictionary<string, List<Vertex>> ToDictionary()
        {
            var dictionary = new Dictionary<string, List<Vertex>>();
            foreach (var entry in Entries)
            {
                dictionary[entry.Key] = entry.Value;
            }
            return dictionary;
        }
    }
}