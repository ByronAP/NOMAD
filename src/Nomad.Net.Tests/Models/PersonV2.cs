namespace Nomad.Net.Tests.Models
{
    using Nomad.Net.Attributes;

    /// <summary>
    /// Extended person model containing an additional field.
    /// </summary>
    public sealed class PersonV2
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

        /// <summary>
        /// Gets or sets an extra value not present in <see cref="Person"/>.
        /// </summary>
        [NomadField(3)]
        public string? Extra { get; set; }
    }
}
