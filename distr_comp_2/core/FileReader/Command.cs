using JetBrains.Annotations;

namespace core.FileReader
{
    public class Command
    {
        public Command(
            int time,
            CommandType type,
            string key = null,
            int? value = null)
        {
            Time = time;
            Type = type;
            Key = key;
            Value = value;
        }

        public int Time { get; }

        public CommandType Type { get; }

        [CanBeNull]
        public string Key { get; }

        [CanBeNull]
        public int? Value { get; }
    }
}