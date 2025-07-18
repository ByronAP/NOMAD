namespace Nomad.Net.Serialization
{
    /// <summary>
    /// Writes the NOMAD binary format.
    /// </summary>
    public interface INomadWriter
    {
        /// <summary>
        /// Writes a field header.
        /// </summary>
        /// <param name="fieldId">The field identifier.</param>
        void WriteFieldHeader(int fieldId);

        /// <summary>
        /// Writes a value of the specified type.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="type">The runtime type of the value.</param>
        void WriteValue(object? value, Type type);
    }
}
