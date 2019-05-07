using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace core.FileReader
{
    public class TimerExecutor
    {
        private ReadOnlyCollection<Command> _commands;
        private Func<Command, Task> _commandCallback;

        private int _commandPos;

        private LinkedList<Task> _startedTasks;

        public TimerExecutor(Command[] commands, Func<Command, Task> commandCallback)
        {
            _commands = new ReadOnlyCollection<Command>(commands.OrderBy(x => x.Time).ToArray());
            _commandCallback = commandCallback;
            _startedTasks = new LinkedList<Task>();
            _commandPos = 0;
        }

        public async Task Execute()
        {
            _startedTasks.Clear();
            if (!_commands.Any())
                return;
            while (_commandPos < _commands.Count)
            {
                var waitTask = _commandPos + 1 < _commands.Count
                    ? Task.Delay(_commands[_commandPos + 1].Time - _commands[_commandPos].Time)
                    : null;

                _startedTasks.AddLast(_commandCallback(_commands[_commandPos]));
                _commandPos++;

                if (waitTask != null)
                    await waitTask;
            }

            await Task.WhenAll(_startedTasks.ToArray());
        }
    }
}