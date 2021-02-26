using System;
using System.Collections.Generic;
using System.Linq;

namespace Draws.CLI {
    public class CommandParser {
        private IEnumerable<ICommand> _commands;
        private IOutputHandler _outputHandler;

        public CommandParser(IEnumerable<ICommand> commands, IOutputHandler outputHandler) {
            _commands = commands;
            _outputHandler = outputHandler;
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

            IEnumerable<Attribute> attributes = command.GetType().GetCustomAttributes(true).Where(x => x is Attribute).Select(x => x as Attribute);
            CommandAttribute commandInfo = attributes.FirstOrDefault(x => x is CommandAttribute) as CommandAttribute;
            IEnumerable<ArgumentAttribute> argumentAttributes = attributes.Where(x => x is ArgumentAttribute).Select(x => x as ArgumentAttribute);

            if (commandInfo.CommandName == verb) {
                try {
                    if (commandInfo.IsSingleArgument)
                        args[0] = $"--{verb}";

                    command.SetArguments(DetermineArguments(argumentAttributes, args));
                    return true;
                }
                catch (Exception e) {
                    if (e is ArgumentException) {
                        _outputHandler.Output(e.Message);
                        return true;
                    }

                    return false;
                }
            }

            return false;
        }

        public void Parse(string[] args) {
            foreach (ICommand cmd in _commands) {
                if (IsCommandCorrect(cmd, args)) {
                    _outputHandler.Output(cmd.RunCommand());
                    return;
                }
            }

            _outputHandler.Output("No command with that name was found.");
            throw new ArgumentException();
        }
    }
}