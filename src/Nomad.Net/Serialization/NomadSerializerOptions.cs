namespace Nomad.Net.Serialization
{
    /// <summary>
    /// Provides configuration for <see cref="NomadSerializer"/>.
    /// </summary>
    public sealed class NomadSerializerOptions
    {
        /// <summary>
        /// Gets or sets the converters used for custom serialization.
        /// </summary>
        public IList<INomadConverter> Converters { get; } = new List<INomadConverter>();

        /// <summary>
        /// Gets or sets a value indicating whether to require explicit <see cref="Attributes.NomadFieldAttribute"/> annotations.
        /// </summary>
        public bool RequireFieldAttribute { get; set; }

        /// <summary>
        /// Gets or sets the resolver used to discover serializable members for a type.
        /// </summary>
        public INomadTypeInfoResolver TypeInfoResolver { get; set; } = new ReflectionNomadTypeInfoResolver();
    }
}
