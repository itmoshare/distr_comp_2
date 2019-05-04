using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace core.FileReader
{
    public class CommandFileReader : IDisposable
    {
        private readonly StreamReader _reader;

        public CommandFileReader(string path)
        {
            _reader = new StreamReader(path);
        }

        public IEnumerable<Command> ReadCommands()
        {
            string line;
            while ((line = _reader.ReadLine()) != null)
                yield return Parse(line);
        }

        private static Command Parse(string line)
        {
            var data = line.Split(new []{ " ", ", " }, StringSplitOptions.RemoveEmptyEntries).ToArray();

            var time = int.Parse(data[0]);

            switch (Enum.Parse<CommandType>(data[1], true))
            {
                case CommandType.Insert:
                    return new Command(time, CommandType.Insert, data[2], int.Parse(data[3]));

                case CommandType.Select:
                    if (data.Length == 2)
                        return new Command(time, CommandType.Select);

                    return new Command(time, CommandType.Select, data[2]);

                default:
                    throw new ArgumentException("Invalid input");
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _reader.Dispose();
        }
    }
}