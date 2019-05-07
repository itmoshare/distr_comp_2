using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using core.FileReader;
using core.Storage;

namespace PeerWorker
{
    public class Executor
    {
        private readonly Storage _storage;
        private HttpClient _httpClient = new HttpClient();

        public bool IsInitialized;

        private Task _mainTask;

        private TimerExecutor _timerExecutor;

        private Task _executeTask;

        public Executor(Storage storage)
        {
            _storage = storage;
        }

        public void Initialize()
        {
            var commandsReader1 = new CommandFileReader($"file{Program.Node}.txt");
            _timerExecutor = new TimerExecutor(
                commandsReader1.ReadCommands().ToArray(),
                command =>
                {
                    //Console.WriteLine($"Executing command {command.Time}...");
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

            IsInitialized = true;

            _mainTask = Task.Run(async () =>
            {
                await WaitInitAsync();

                Console.WriteLine("Start executing...");
                await _timerExecutor.Execute();
            });
        }

        private async Task WaitInitAsync()
        {
            while (true)
            {
                try
                {
                    var slave1 = await _httpClient.GetAsync($"{Topology.Peer1}api/monitoring/ping");
                    var slave2 = await _httpClient.GetAsync($"{Topology.Peer2}api/monitoring/ping");
                    var slave3 = await _httpClient.GetAsync($"{Topology.Peer3}api/monitoring/ping");
                    if (slave1.StatusCode == HttpStatusCode.OK && slave2.StatusCode == HttpStatusCode.OK && slave3.StatusCode == HttpStatusCode.OK)
                    {
                        break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine($"{Program.Node}: waiting nodes...");
                    await Task.Delay(20);
                }
            }
        }

        private Task SelectAsync(string key)
        {
            var node = GetNode(key);
            if (node == 1)
            {
                return _httpClient.GetAsync($"{Topology.Peer1}api/data/{key}");
            }
            if (node == 2)
            {
                return _httpClient.GetAsync($"{Topology.Peer2}api/data/{key}");
            }
            if (node == 3)
            {
                return _httpClient.GetAsync($"{Topology.Peer3}api/data/{key}");
            }
            throw new ArgumentException();
        }

        private Task InsertAsync(string key, long data)
        {
            var node = GetNode(key);
            if (node == 1)
            {
                return _httpClient.PutAsync($"{Topology.Peer1}api/data/{key}?data={data}", null);
            }
            if (node == 2)
            {
                return _httpClient.PutAsync($"{Topology.Peer2}api/data/{key}?data={data}", null);
            }
            if (node == 3)
            {
                return _httpClient.PutAsync($"{Topology.Peer3}api/data/{key}?data={data}", null);
            }
            throw new ArgumentException();
        }

        private Task SelectAllAsync()
        {
            return Task.WhenAll(
                _httpClient.GetAsync($"{Topology.Peer1}api/data"),
                _httpClient.GetAsync($"{Topology.Peer2}api/data"),
                _httpClient.GetAsync($"{Topology.Peer3}api/data"));
        }

        private Task ShowStatsAsync()
        {
            return Task.WhenAll(
                _httpClient.GetAsync($"{Topology.Peer1}api/monitoring/stats"),
                _httpClient.GetAsync($"{Topology.Peer2}api/monitoring/stats"),
                _httpClient.GetAsync($"{Topology.Peer3}api/monitoring/stats"));
        }

        private static int GetNode(string key) => (int) ((uint)key.GetHashCode() % 3 + 1);
    }
}