using System;

namespace Oogi2.Attributes
{
    /// <summary>
    /// Entity type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class EntityType : Attribute
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Oogi2.Attributes.EntityType"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="value">Value.</param>
        public EntityType(string name, object @value) : this(name, @value.ToString())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Oogi2.Attributes.EntityType"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="value">Value.</param>
        public EntityType(string name, string @value)
        {
            Name = name;
            Value = @value;
        }
    }
}
