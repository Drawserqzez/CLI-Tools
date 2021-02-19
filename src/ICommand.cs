using System;
using System.Collections.Generic;

namespace Draws.CLI {
    public interface ICommand {
        string RunCommand();
        void SetArguments(Dictionary<string, string> args);
    }
}