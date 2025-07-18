using System;

namespace Nomad.Net.Attributes
{
    /// <summary>
    /// Specifies the NOMAD field identifier for a member.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class NomadFieldAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NomadFieldAttribute"/> class.
        /// </summary>
        /// <param name="fieldId">The integer field identifier.</param>
        public NomadFieldAttribute(int fieldId)
        {
            FieldId = fieldId;
        }

        /// <summary>
        /// Gets the NOMAD field identifier.
        /// </summary>
        public int FieldId { get; }
    }
}
