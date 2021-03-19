using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Draws.CLI.Tests
{
    public class ParserTests
    {
        [Command("echo", "This echoes the first argument", isSingleArgument: true)]
        [Argument("echo", "The string that is echoed.", isFlag: false, required: true, 'e')]
        private class EchoCommand : ICommand {
            private Dictionary<string, string> _args;
            
            public string RunCommand()
            {
                return _args.First().Value;
            }

            public void SetArguments(Dictionary<string, string> args)
            {
                _args = args;
            }
        }

        private class TestOutputHandler : IOutputHandler
        {
            public Action<string> OutputAction { get; set; }

            public void Output(string output) {
                OutputAction(output);
            }
        }

        [Fact]
        public void Does_parser_use_correct_command_with_long_name() {
            // Arrange
            string result = "";
            Action<string> outputAction = (string output) => result = output;
            EchoCommand testCommand = new EchoCommand();
            TestOutputHandler output = new TestOutputHandler();
            output.OutputAction = outputAction;

            CommandParser sut = new CommandParser(new [] { testCommand }, output);
            // Act
            sut.Parse(new[] { "echo", "Echoed string" });

            // Assert
            result.Should().Be("Echoed string");
        }
    }
}
