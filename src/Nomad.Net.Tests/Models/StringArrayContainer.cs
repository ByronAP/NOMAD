namespace Nomad.Net.Tests.Models
{
    using Nomad.Net.Attributes;

    /// <summary>
    /// Model containing a string array for testing purposes.
    /// </summary>
    public sealed class StringArrayContainer
    {
        /// <summary>
        /// Gets or sets the values.
        /// </summary>
        [NomadField(1)]
        public string[]? Values { get; set; }
    }
}
