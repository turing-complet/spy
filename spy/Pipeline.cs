using spy.format;
using spy.input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace spy
{
    // TODO: export config from running pipeline?
    public class Pipeline
    {
        private List<IInput<IFormat>> inputs;
        public Pipeline(Config config)
        {
            var inputs = new List<IInput<IFormat>>();
            var inputInfo = GetTypesWithAttribute(typeof(SpyInput));
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

                inputs.Add((IInput<IFormat>)instance);
            }
        }

        private void SetProperty(object instance, InputPlugin settings, PropertyInfo prop)
        {
            SetValue(prop, instance, settings[prop.Name]);
        }

        private void SetValue(PropertyInfo prop, object instance, object value)
        {
            prop.SetValue(instance, Convert.ChangeType(value, prop.PropertyType));
        }

        private static IEnumerable<PropertyInfo> GetSettingsForPlugin(Type inputType)
        {
            var properties = inputType.GetProperties(BindingFlags.Public)
                .Where(t => t.CustomAttributes.Any(a => a.AttributeType == typeof(SpySetting)));
            return properties;
        }

        private static IEnumerable<(Type type, string name)> GetTypesWithAttribute(Type attrType)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            foreach (Type type in assembly.GetTypes())
            {
                var attr = type.GetCustomAttributes(attrType, true).FirstOrDefault();
                if (attr != null)
                {
                    yield return (type, (string) type.GetProperty("name").GetValue(attr));
                }
            }
        }
    }
}
