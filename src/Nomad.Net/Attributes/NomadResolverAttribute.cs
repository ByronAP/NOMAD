using System;

namespace Nomad.Net.Attributes
{
    /// <summary>
    /// Specifies the fully qualified name of an <see cref="Serialization.INomadTypeInfoResolver"/>
    /// to use for the annotated type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed class NomadResolverAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NomadResolverAttribute"/> class.
        /// </summary>
        /// <param name="resolverType">The type name of the resolver.</param>
        public NomadResolverAttribute(string resolverType)
        {
            ResolverType = resolverType;
        }

        /// <summary>
        /// Gets the fully qualified resolver type name.
        /// </summary>
        public string ResolverType { get; }
    }
}
