using System;

namespace Nomad.Net.Attributes
{
    /// <summary>
    /// Indicates that a member should be ignored during NOMAD serialization and deserialization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class NomadIgnoreAttribute : Attribute
    {
    }
}
