using System;
using System.Collections.Generic;
using System.Linq;

namespace Draws.CLI {
    public class CommandParser {
        private IEnumerable<ICommand> _commands;
        private Action<string> _output; 

        public CommandParser(IEnumerable<ICommand> commands, Action<string> outputAction) {
            _commands = commands;
            _output = outputAction;
        }

        private Dictionary<string, string> DetermineArguments(IEnumerable<ArgumentAttribute> argumentAttributes, string[] args) {
            Dictionary<string, string> commandArguments = new Dictionary<string, string>();
            IEnumerable<string> arguments = args.Where(x => x.StartsWith('-') || x.StartsWith("--")).Select(x => x.Replace('-', ' ').Trim());

            foreach (var argument in argumentAttributes) {
                string arg = arguments.FirstOrDefault(x => x == argument.ArgumentName || x == argument.ShortName.ToString());

                if (arg == null && argument.Required) 
                    throw new ArgumentException($"This argument was not specified: {argument.ArgumentName}\nHint: {argument.HelpText}");

                var argsList = args.ToList();

                if (!argument.IsFlag) {
                    int argIndex = (argsList.IndexOf("--" + arg) == -1) ? argsList.IndexOf("-" + arg) : argsList.IndexOf("--" + arg);
                    string value = "";

                    try {
                        value = args[argIndex + 1];
                    }
                    catch {
                        throw new ArgumentException($"The argument {argument.ArgumentName} did not have a specified value.\nHint: {argument.HelpText}");
                    }

                    commandArguments.Add(argument.ArgumentName, value);
                }
                else {
                    commandArguments.Add(argument.ArgumentName, "true");
                }
            }

            return commandArguments;
        }

        private bool IsCommandCorrect(ICommand command, string[] args) {
            string verb = args[0];

            // BUG: command.GetType().GetCustomAttributes() returns null somehow?
            Attribute[] attributes = (command.GetType().GetCustomAttributes(true) as Attribute[]).ToArray();
            CommandAttribute commandInfo = attributes.FirstOrDefault(x => x is CommandAttribute) as CommandAttribute;
            IEnumerable<ArgumentAttribute> argumentAttributes = attributes.Where(x => x is ArgumentAttribute) as IEnumerable<ArgumentAttribute>;

            if (commandInfo.CommandName == verb) {
                try {
                    command.SetArguments(DetermineArguments(argumentAttributes, args));
                    return true;
                }
                catch (Exception e) {
                    if (e is ArgumentException) {
                        _output.Invoke(e.Message);
                        return true;
                    }

                    return false;
                }
            }

            return false;
        }

        public ICommand Parse(string[] args) {
            foreach (ICommand cmd in _commands) {
                if (IsCommandCorrect(cmd, args))
                    return cmd;
            }

            _output.Invoke("No command with that name was found.");
            throw new ArgumentException();
        }
    }
}