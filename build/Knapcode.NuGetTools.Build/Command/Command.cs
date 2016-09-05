using System;
using System.Collections.Generic;
using System.IO;

namespace Knapcode.NuGetTools.Build
{
    public class Command
    {
        public Command(string fileName, string arguments)
        {
            FileName = fileName;
            Arguments = arguments;
            WorkingDirectory = Directory.GetCurrentDirectory();
            Environment = new Dictionary<string, string>();
            Timeout = TimeSpan.FromMinutes(1);
        }

        public Command(string fileName) : this(fileName, string.Empty)
        {
        }

        public string WorkingDirectory { get; set; }
        public string FileName { get; set; }
        public string Arguments { get; set; }
        public TimeSpan Timeout { get; set; }
        public IDictionary<string, string> Environment { get; set; }

        public string GetDisplayString()
        {
            if (string.IsNullOrEmpty(Arguments))
            {
                return FileName;
            }

            return $"{FileName} {Arguments}";
        }
    }
}
