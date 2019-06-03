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
}
