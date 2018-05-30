using System;

namespace Oogi2.Attributes
{
    /// <summary>
    /// Entity type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class EntityTypeAttribute : Attribute
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
        /// Initializes a new instance of the <see cref="T:Oogi2.Attributes.EntityTypeAttribute"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="value">Value.</param>
        public EntityTypeAttribute(string name, object @value) : this(name, @value.ToString())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Oogi2.Attributes.EntityTypeAttribute"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="value">Value.</param>
        public EntityTypeAttribute(string name, string @value)
        {
            Name = name;
            Value = @value;
        }
    }
}