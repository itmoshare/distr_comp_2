using System.Collections.Concurrent;
using System.Linq;

namespace core.Storage
{
    public class Storage
    {
        private readonly ConcurrentDictionary<string, long> _data;

        public Storage()
        {
            _data = new ConcurrentDictionary<string, long>();
        }

        public void Insert(string key, long value)
        {
            _data[key] = value;
        }

        public StorageItem[] Select()
        {
            return _data.Select(x => new StorageItem(x.Key, x.Value)).ToArray();
        }

        public StorageItem Select(string key)
        {
            return _data.TryGetValue(key, out var res)
                ? new StorageItem(key, res)
                : null;
        }
    }
}