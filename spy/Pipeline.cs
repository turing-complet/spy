using spy.format;
using spy.input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Collections.Concurrent;
using spy.output;

namespace spy
{
    // TODO: export config from running pipeline?
    public class Pipeline
    {
        private Dictionary<IInput<IFormat>, ConcurrentQueue<IFormat>> _inputs;
        private Dictionary<ConcurrentQueue<IFormat>, Output<IFormat>> _outputs;
        
        public Pipeline(Config config)
        {
            _inputs = new Dictionary<IInput<IFormat>, ConcurrentQueue<IFormat>>();
            _outputs = new Dictionary<ConcurrentQueue<IFormat>, Output<IFormat>>();

            var inputInfo = typeof(SpyInput).GetMatchingTypes();
            foreach(var info in inputInfo)
            {
                var instance = Activator.CreateInstance(info.type);
                string configKey = (
                    info.type.GetCustomAttributes()
                    .Where(t => t.GetType() == typeof(SpyInput))
                    .FirstOrDefault() as SpyInput
                    ).name;
                    
                foreach (var prop in GetSettingsForPlugin(info.type))
                {
                    SetProperty(instance, config.Inputs[info.name], prop);
                }

                _inputs[(IInput<IFormat>)instance] = new ConcurrentQueue<IFormat>();
            }
        }

        public void Run()
        {
            foreach (var input in _inputs.Keys)
            {
                var q = _inputs[input]; // does this work, or need to define key
                input.Start((IFormat s) => q.Enqueue(s));
                _outputs[q].Start(q);
            }
        }

        private void SetProperty(object instance, InputPlugin settings, PropertyInfo prop)
        {
            object configValue = settings[prop.Name];
            prop.SetValue(instance, Convert.ChangeType(configValue, prop.PropertyType));
        }

        private static IEnumerable<PropertyInfo> GetSettingsForPlugin(Type inputType)
        {
            var properties = inputType.GetProperties(BindingFlags.Public)
                .Where(t => t.CustomAttributes.Any(a => a.AttributeType == typeof(SpySetting)));
            return properties;
        }
    }
}
