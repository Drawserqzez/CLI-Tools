using System;

namespace Draws.CLI {
    public class ArgumentAttribute : Attribute {
        public string ArgumentName { get; private set; }
        public string HelpText { get; private set; }
        public bool IsFlag { get; private set; } = false;
        public char ShortName { get; private set; }
        public bool Required { get; private set; } = false;

        public ArgumentAttribute(string argumentName, string helpText, bool isFlag = false, bool required = false, char shortName = ' ') {
            ArgumentName = argumentName;
            HelpText = helpText;
            IsFlag = isFlag;
            ShortName = (Char.IsWhiteSpace(shortName)) ? argumentName[0] : shortName;
            Required = required;
        }
    }
}