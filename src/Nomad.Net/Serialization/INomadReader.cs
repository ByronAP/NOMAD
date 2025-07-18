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
        /// Reads a structural token from the stream.
        /// </summary>
        /// <returns>The token read.</returns>
        NomadToken ReadToken();

        /// <summary>
        /// Peeks at the next structural token without consuming it.
        /// </summary>
        /// <returns>The next token.</returns>
        NomadToken PeekToken();

        /// <summary>
        /// Reads a value of the specified type.
        /// </summary>
        /// <param name="type">The expected type.</param>
        /// <returns>The value read from the stream.</returns>
        object? ReadValue(Type type);
    }
}
