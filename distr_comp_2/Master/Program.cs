using System;
using System.Linq;
using System.Net.Http;
using core.FileReader;

namespace Master
{
    class Program
    {
        private static HttpClient _httpClient = new HttpClient();

        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                throw new ArgumentException();
            }

            var filePath1 = args[0];
            var filePath2 = args[1];
            var filePath3 = args[3];
            var commandsReader1 = new CommandFileReader(filePath1);
            var commandsReader2 = new CommandFileReader(filePath2);
            var commandsReader3 = new CommandFileReader(filePath3);
            var executor = new TimerExecutor(
                commandsReader1.ReadCommands()
                .Concat(commandsReader2.ReadCommands())
                .Concat(commandsReader3.ReadCommands())
                .ToArray(),
                command =>
                {

                });
        }

        private static void Insert(int node, string key, long data)
        {
            if (node == 1)
            {
                _httpClient.PutAsync($"api/data/{key}", new HttpConte data);
            }
        }
    }
}