using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace core.FileReader
{
    public class TimerExecutor : IDisposable
    {
        private ReadOnlyCollection<Command> _commands;
        private Func<Command, Task> _commandCallback;

        private readonly Timer _timer;
        private int _commandPos;

        private ConcurrentBag<Task> _startedTasks;

        public TimerExecutor(Command[] commands, Func<Command, Task> commandCallback)
        {
            _commands = new ReadOnlyCollection<Command>(commands);
            _commandCallback = commandCallback;
            _startedTasks = new ConcurrentBag<Task>();

            _timer = new Timer
            {
                AutoReset = false
            };
            _timer.Elapsed += (_, __) => HandleTick();
            _commandPos = 0;
        }

        public void Start()
        {
            if (_commands.Any())
                RegisterNextCommand(0);
        }

        private void HandleTick()
        {
            _startedTasks.Add(_commandCallback(_commands[_commandPos]));

            if (_commandPos + 1 < _commands.Count)
            {
                RegisterNextCommand(_commandPos + 1);
            }
        }

        private void RegisterNextCommand(int pos)
        {
            _timer.Interval = pos != 0
                ? _commands[pos].Time - _commands[pos - 1].Time
                : _timer.Interval = _commands[0].Time;
            _timer.Start();
        }

        public void Wait()
        {
            Task.WaitAll(_startedTasks.ToArray());
            _startedTasks.Clear();
        }
        
        /// <inheritdoc />
        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}