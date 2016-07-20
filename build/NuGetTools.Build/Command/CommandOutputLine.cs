namespace Knapcode.NuGetTools.Build
{
    public class CommandOutputLine
    {
        public CommandOutputLine(CommandOutputLineType type, string value)
        {
            Type = type;
            Value = value;
        }

        public CommandOutputLineType Type { get; set; }
        public string Value { get; set; }
    }
}
