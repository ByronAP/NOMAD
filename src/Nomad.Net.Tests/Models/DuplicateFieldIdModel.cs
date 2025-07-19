namespace Nomad.Net.Tests.Models
{
    using Nomad.Net.Attributes;

    /// <summary>
    /// Model containing duplicate field identifiers for validation tests.
    /// </summary>
    public sealed class DuplicateFieldIdModel
    {
        /// <summary>
        /// Gets or sets the first value.
        /// </summary>
        [NomadField(1)]
        public int Value1 { get; set; }

        /// <summary>
        /// Gets or sets the second value.
        /// </summary>
        [NomadField(1)]
        public int Value2 { get; set; }
    }
}
