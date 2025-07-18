using System.Reflection;
namespace Nomad.Net.Serialization
{
    /// <summary>
    /// Resolves the serializable members for a given type.
    /// </summary>
    public interface INomadTypeInfoResolver
    {
        /// <summary>
        /// Gets the members that should be serialized for the specified type.
        /// </summary>
        /// <param name="type">The type to inspect.</param>
        /// <returns>The members that should be serialized.</returns>
        IReadOnlyList<MemberInfo> GetSerializableMembers(Type type);
    }
}
