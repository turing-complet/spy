using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace spy
{
    // TODO: hot reload, validation
    public class Config
    {
        public Dictionary<string, InputPlugin> Inputs { get; set; }
        public Dictionary<string, OutputPlugin> Outputs { get; set; }

        public static Config LoadFrom(string configFile)
        {
            string text = File.ReadAllText(configFile);
            return JsonConvert.DeserializeObject<Config>(text);
        }

        public List<string> Validate()
        {
            var errors = new List<string>();
            foreach (var key in Inputs.Keys)
            {
                if (!Outputs.Keys.Any(o => o == Inputs[key].Output))
                {
                    errors.Add($"Input {key} has invalid output type = {Inputs[key].Output ?? "null"}");
                }
            }
            return errors;
        }

    }

    public class InputPlugin : Plugin
    {
        public string Output => (string)this["output"];
    }

    public class OutputPlugin : Plugin { }

    public class Plugin : Dictionary<string, object>
    {
        //[JsonExtensionData]
        //public Dictionary<string, object> Settings { get; set; }

        public string Type { get; set; } // textfile, http, console, etc
    }
}
