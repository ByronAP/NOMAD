namespace Nomad.Net.Attributes
{
    /// <summary>
    /// Indicates that a member should be ignored during NOMAD serialization and deserialization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed class NomadIgnoreAttribute : Attribute
    {
    }
}
