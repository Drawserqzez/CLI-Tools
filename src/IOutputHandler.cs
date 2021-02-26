using System;

namespace Draws.CLI {
    public interface IOutputHandler {
        Action<string> OutputAction { get; }
        void Output(string output);
    }
}