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
            if (_reader.EndOfStream)
                yield break;

            yield return Parse(_reader.ReadLine());
        }

        private static Command Parse(string line)
        {
            var data = line.Split(new []{ " ", ", " }, StringSplitOptions.RemoveEmptyEntries).ToArray();

            var time = int.Parse(data[0]);

            switch (Enum.Parse<CommandType>(data[1]))
            {
                case CommandType.Insert:
                    return new Command(time, CommandType.Insert, data[2], int.Parse(data[3]));

                case CommandType.Select:
                    if (data.Length == 2)
                        return new Command(time, CommandType.Select);

                    return new Command(time, CommandType.Select, data[3]);

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