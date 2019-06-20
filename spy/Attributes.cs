using System;
using System.Collections.Generic;
using System.Text;

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
}
