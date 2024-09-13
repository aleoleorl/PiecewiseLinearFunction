using System.Collections.ObjectModel;

namespace PLF_WPForiented.model
{
    [Serializable]
    public class DictionaryEntry
    {
        public string Key { get; set; }
        public ObservableCollection<Vertex> Value { get; set; }

        public DictionaryEntry() 
        { 
        }

        public DictionaryEntry(string key, ObservableCollection<Vertex> value)
        {
            Key = key;
            Value = value;
        }
    }

    [Serializable]
    public class SerializableDictionary
    {
        public List<DictionaryEntry> Entries { get; set; } = new List<DictionaryEntry>();

        public SerializableDictionary()
        {
        }

        public SerializableDictionary(Dictionary<string, ObservableCollection<Vertex>> dictionary)
        {
            foreach (var entry in dictionary)
            {
                Entries.Add(new DictionaryEntry(entry.Key, entry.Value));
            }
        }

        public Dictionary<string, ObservableCollection<Vertex>> ToDictionary()
        {
            var dictionary = new Dictionary<string, ObservableCollection<Vertex>>();
            foreach (var entry in Entries)
            {
                dictionary[entry.Key] = entry.Value;
            }
            return dictionary;
        }
    }
}