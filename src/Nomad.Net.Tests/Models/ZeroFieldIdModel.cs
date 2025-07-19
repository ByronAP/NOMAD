namespace Nomad.Net.Tests.Models
{
    using Nomad.Net.Attributes;

    /// <summary>
    /// Model containing a field with an invalid zero identifier.
    /// </summary>
    public sealed class ZeroFieldIdModel
    {
        /// <summary>
        /// Gets or sets the value with an invalid identifier.
        /// </summary>
        [NomadField(0)]
        public int Value { get; set; }
    }
}
