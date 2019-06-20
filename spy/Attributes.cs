using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;

namespace spy
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SpyInput : Attribute
    {
        public string name;
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class SpyOutput : Attribute
    {
        public string name;
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class SpySetting : Attribute
    {
        public string name;
        public string description;
    }

    public static class AttributeExtensions
    {
        public static IEnumerable<(Type type, string name)> GetMatchingTypes(this Type attrType)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            foreach (Type type in assembly.GetTypes())
            {
                var attr = type.GetCustomAttributes(attrType, true).FirstOrDefault();
                if (attr != null)
                {
                    yield return (type, (string) attrType.GetField("name").GetValue(attr));
                }
            }
        }
    }
}
