namespace Nomad.Net.Serialization
{
    /// <summary>
    /// Defines methods to convert between managed objects and the NOMAD representation.
    /// </summary>
    public interface INomadConverter
    {
        /// <summary>
        /// Determines whether this converter can handle the specified type.
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <returns><c>true</c> if the converter can serialize the type; otherwise, <c>false</c>.</returns>
        bool CanConvert(Type type);

        /// <summary>
        /// Writes the specified value using the provided writer.
        /// </summary>
        /// <param name="writer">The <see cref="INomadWriter"/> to write to.</param>
        /// <param name="value">The value to write.</param>
        void Write(INomadWriter writer, object? value);

        /// <summary>
        /// Reads a value using the provided reader.
        /// </summary>
        /// <param name="reader">The <see cref="INomadReader"/> to read from.</param>
        /// <param name="type">The target type.</param>
        /// <returns>The deserialized value.</returns>
        object? Read(INomadReader reader, Type type);
    }
}
