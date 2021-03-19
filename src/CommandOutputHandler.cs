using System;

namespace Draws.CLI {
    public class CommandOutputHandler : IOutputHandler {
        private readonly Action<string> _outputAction;

        public Action<string> OutputAction { 
            get {
                return _outputAction;
            }
        }

        public CommandOutputHandler() {
            _outputAction = Console.WriteLine;
        }

        public void Output(string output) {
            _outputAction.Invoke(output);
        }
    }
}