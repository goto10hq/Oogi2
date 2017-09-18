using System;

namespace Oogi2.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class EntityType : Attribute
    {
        public string Name { get; }
        public string Value { get; }

        public EntityType(string name, object value) : this(name, value.ToString())
        {
        }

        public EntityType(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
