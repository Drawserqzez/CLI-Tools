using System;

namespace Draws.CLI {
    public class CommandAttribute : Attribute {
        public string CommandName { get; private set; }
        public string HelpText { get; private set; }
        public bool IsSingleArgument { get; private set; }

        public CommandAttribute(string commandName, string helpText, bool isSingleArgument = false) {

            CommandName = commandName;
            HelpText = helpText;
            IsSingleArgument = isSingleArgument;
        }
    }
}