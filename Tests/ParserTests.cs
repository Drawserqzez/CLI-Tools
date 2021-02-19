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
        [Argument("echo", "The string that is echoed.", false, true, 'e')]
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

        [Fact]
        public void Does_parser_use_correct_command_with_long_name() {
            // Arrange
            EchoCommand testCommand = new EchoCommand();
            string result = "";
            CommandParser sut = new CommandParser(new [] { testCommand }, (string output) => result = output);
            // Act
            sut.Parse(new[] { "echo", "Echoed string" }).RunCommand();

            // Assert
            result.Should().Be("Echoed string");
        }
    }
}
