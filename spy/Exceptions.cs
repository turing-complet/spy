using System;
using System.Collections.Generic;
using System.Text;

namespace spy
{
    public class ConfigException : Exception
    {
        public ConfigException(List<string> errors)
            : base($"Errors in config file: {string.Join(Environment.NewLine, errors)}")
        {
        }
    }
}
