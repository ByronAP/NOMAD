namespace Nomad.Net.Tests.Models
{
    using Nomad.Net.Attributes;

    /// <summary>
    /// Model containing a value of type <see cref="object"/> for dynamic tests.
    /// </summary>
    public sealed class ObjectContainer
    {
        /// <summary>
        /// Gets or sets the arbitrary value.
        /// </summary>
        [NomadField(1)]
        public object? Value { get; set; }
    }
}
