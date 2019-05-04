using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using core.FileReader;
using MWCore;

namespace Master
{
    class Program
    {
        private static HttpClient _httpClient = new HttpClient();

        static async Task Main(string[] args)
        {
            if (args.Length != 3)
            {
                throw new ArgumentException();
            }

            var filePath1 = args[0];
            var filePath2 = args[1];
            var filePath3 = args[2];
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
                    Console.WriteLine($"Executing command {command.Time}...");
                    switch (command.Type)
                    {
                        case CommandType.Insert:
                            // ReSharper disable once PossibleInvalidOperationException
                            return InsertAsync(command.Key, command.Value.Value);

                        case CommandType.Select:
                            return command.Key != null
                                ? SelectAsync(command.Key)
                                : SelectAllAsync();

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                });

            await WaitSlavesAsync();

            Console.WriteLine("Start executing...");
            await executor.Execute();

            Console.WriteLine("Wait residual execution...");
            await executor.WaitComplete();

            Console.WriteLine("Show stats...");
            await ShowStatsAsync();

            Console.WriteLine("Done!");
        }

        private static async Task WaitSlavesAsync()
        {
            while (true)
            {
                try
                {
                    var slave1 = await _httpClient.GetAsync($"{Topology.Slave1}/api/monitoring/ping");
                    var slave2 = await _httpClient.GetAsync($"{Topology.Slave2}/api/monitoring/ping");
                    if (slave1 != null && slave2 != null)
                    {
                        break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Master: wait slaves...");
                    await Task.Delay(100);
                }
            }
        }

        private static Task SelectAsync(string key)
        {
            var node = GetNode(key);
            if (node == 1)
            {
                return _httpClient.GetAsync($"{Topology.Slave1}api/data/{key}");
            }
            if (node == 2)
            {
                return _httpClient.GetAsync($"{Topology.Slave2}api/data/{key}");
            }
            throw new ArgumentException();
        }

        private static Task InsertAsync(string key, long data)
        {
            var node = GetNode(key);
            if (node == 1)
            {
                return _httpClient.PutAsync($"{Topology.Slave1}api/data/{key}?data={data}", null);
            }
            if (node == 2)
            {
                return _httpClient.PutAsync($"{Topology.Slave2}api/data/{key}?data={data}", null);
            }
            throw new ArgumentException();
        }

        private static Task SelectAllAsync()
        {
            return Task.WhenAll(
                _httpClient.GetAsync($"{Topology.Slave1}api/data"),
                _httpClient.GetAsync($"{Topology.Slave2}api/data"));
        }

        private static Task ShowStatsAsync()
        {
            return Task.WhenAll(
                _httpClient.GetAsync($"{Topology.Slave1}api/monitoring/stats"),
                _httpClient.GetAsync($"{Topology.Slave2}api/monitoring/stats"));
        }

        private static int GetNode(string key) => (int) ((uint)key.GetHashCode() % 2 + 1);
    }
}