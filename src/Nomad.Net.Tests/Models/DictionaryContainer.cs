using System.Collections.Generic;
using Nomad.Net.Attributes;

namespace Nomad.Net.Tests.Models
{
    /// <summary>
    /// Model containing a dictionary for serialization tests.
    /// </summary>
    public sealed class DictionaryContainer
    {
        /// <summary>
        /// Gets or sets the values keyed by string.
        /// </summary>
        [NomadField(1)]
        public Dictionary<string, int>? Values { get; set; }
    }
}
