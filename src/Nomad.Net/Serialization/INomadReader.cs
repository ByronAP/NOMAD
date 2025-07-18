using System;

namespace Nomad.Net.Serialization
{
    /// <summary>
    /// Reads the NOMAD binary format.
    /// </summary>
    public interface INomadReader
    {
        /// <summary>
        /// Reads a field header.
        /// </summary>
        /// <returns>The field identifier, or <c>null</c> if there are no more fields.</returns>
        int? ReadFieldHeader();

        /// <summary>
        /// Reads a value of the specified type.
        /// </summary>
        /// <param name="type">The expected type.</param>
        /// <returns>The value read from the stream.</returns>
        object? ReadValue(Type type);
    }
}
