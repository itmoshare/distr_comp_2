namespace core.Storage
{
    public class StorageItem
    {
        public StorageItem(string key, long value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; }
        public long Value { get; }
    }
}