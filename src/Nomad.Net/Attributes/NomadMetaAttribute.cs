namespace Nomad.Net.Attributes
{
    /// <summary>
    /// Provides an extensible mechanism to attach custom metadata to a member.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public sealed class NomadMetaAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NomadMetaAttribute"/> class.
        /// </summary>
        /// <param name="name">The metadata name.</param>
        /// <param name="value">The metadata value.</param>
        public NomadMetaAttribute(string name, string value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Gets the metadata name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the metadata value.
        /// </summary>
        public string Value { get; }
    }
}
