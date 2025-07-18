namespace Nomad.Net.Tests.Models
{
    using Nomad.Net.Attributes;

    /// <summary>
    /// Model containing an integer array for testing purposes.
    /// </summary>
    public sealed class IntArrayContainer
    {
        /// <summary>
        /// Gets or sets the values.
        /// </summary>
        [NomadField(1)]
        public int[]? Values { get; set; }
    }
}
