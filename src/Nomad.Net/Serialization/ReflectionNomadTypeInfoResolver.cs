using System.Reflection;
using System.Linq;

namespace Nomad.Net.Serialization
{
    /// <summary>
    /// Resolves serializable members using runtime reflection.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed class ReflectionNomadTypeInfoResolver : INomadTypeInfoResolver
    {
        /// <inheritdoc />
        public IReadOnlyList<MemberInfo> GetSerializableMembers(Type type)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            return type.GetMembers(flags)
                .Where(m => m.MemberType == MemberTypes.Property || m.MemberType == MemberTypes.Field)
                .Where(m => !Attribute.IsDefined(m, typeof(Attributes.NomadIgnoreAttribute)))
                .ToArray();
        }
    }
}
