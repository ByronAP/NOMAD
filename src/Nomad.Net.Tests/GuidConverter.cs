using System;
using Nomad.Net.Serialization;

namespace Nomad.Net.Tests
{
    /// <summary>
    /// Converter for <see cref="Guid"/> values using string representation.
    /// </summary>
    internal sealed class GuidConverter : INomadConverter
    {
        /// <inheritdoc />
        public bool CanConvert(Type type) => type == typeof(Guid);

        /// <inheritdoc />
        public void Write(INomadWriter writer, object? value)
        {
            writer.WriteValue(((Guid)value!).ToString(), typeof(string));
        }

        /// <inheritdoc />
        public object? Read(INomadReader reader, Type type)
        {
            string? text = (string?)reader.ReadValue(typeof(string));
            return text is null ? null : Guid.Parse(text);
        }
    }
}
