namespace Nomad.Net.Tests.Models
{
    using Nomad.Net.Attributes;

    /// <summary>
    /// Simple person model for serialization tests.
    /// </summary>
    public sealed class Person
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [NomadField(1)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [NomadField(2)]
        public string? Name { get; set; }
    }
}
