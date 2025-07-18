using System;
using Nomad.Net.Attributes;

namespace Nomad.Net.Tests.Models
{
    /// <summary>
    /// Model holding a <see cref="Guid"/> value.
    /// </summary>
    public sealed class GuidContainer
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [NomadField(1)]
        public Guid Id { get; set; }
    }
}
