using System.Collections.Concurrent;
using System.Linq;

namespace core.Storage
{
    public class Storage
    {
        private readonly ConcurrentDictionary<string, long> _data;

        private int _handled = 0;

        public Storage()
        {
            _data = new ConcurrentDictionary<string, long>();
        }

        public void Insert(string key, long value)
        {
            _handled++;
            _data[key] = value;
        }

        public StorageItem[] Select()
        {
            _handled++;
            return _data.Select(x => new StorageItem(x.Key, x.Value)).ToArray();
        }

        public StorageItem Select(string key)
        {
            _handled++;
            return _data.TryGetValue(key, out var res)
                ? new StorageItem(key, res)
                : null;
        }

        public int GetHandledCount() => _handled;
        public int GetItemsCount() => _data.Count;
    }
}