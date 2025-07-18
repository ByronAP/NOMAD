using System.Reflection;
using System.Linq;
using Nomad.Net.Attributes;

namespace Nomad.Net.Serialization
{
    /// <summary>
    /// Serializes and deserializes objects using the NOMAD binary format.
    /// </summary>
    public sealed class NomadSerializer
    {
        private readonly NomadSerializerOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="NomadSerializer"/> class.
        /// </summary>
        /// <param name="options">The serializer options.</param>
        public NomadSerializer(NomadSerializerOptions? options = null)
        {
            _options = options ?? new NomadSerializerOptions();
        }

        /// <summary>
        /// Serializes an object to the provided writer.
        /// </summary>
        /// <param name="writer">The writer to use.</param>
        /// <param name="value">The value to serialize.</param>
        /// <typeparam name="T">The type of the value.</typeparam>
        public void Serialize<T>(INomadWriter writer, T value)
        {
            WriteObject(writer, value, typeof(T));
        }

        /// <summary>
        /// Deserializes an object from the provided reader.
        /// </summary>
        /// <param name="reader">The reader to use.</param>
        /// <typeparam name="T">The target type.</typeparam>
        /// <returns>The deserialized object.</returns>
        public T? Deserialize<T>(INomadReader reader)
        {
            object? result = ReadObject(reader, typeof(T));
            return (T?)result;
        }

        private void WriteObject(INomadWriter writer, object? value, Type type)
        {
            if (value is null)
            {
                writer.WriteValue(null, type);
                return;
            }

            foreach (var member in GetSerializableMembers(type))
            {
                object? memberValue = member switch
                {
                    PropertyInfo p => p.GetValue(value),
                    FieldInfo f => f.GetValue(value),
                    _ => null,
                };

                if (memberValue is null)
                {
                    continue;
                }

                var fieldAttr = member.GetCustomAttribute<NomadFieldAttribute>();
                if (fieldAttr is null && _options.RequireFieldAttribute)
                {
                    continue;
                }

                int fieldId = fieldAttr?.FieldId ?? member.MetadataToken;
                writer.WriteFieldHeader(fieldId);
                WriteValue(writer, memberValue, memberValue.GetType());
            }
        }

        private object? ReadObject(INomadReader reader, Type type)
        {
            object? instance = Activator.CreateInstance(type);
            var members = GetSerializableMembers(type).ToDictionary(m => m.GetCustomAttribute<NomadFieldAttribute>()?.FieldId ?? m.MetadataToken);
            int? fieldId;
            while ((fieldId = reader.ReadFieldHeader()) != null)
            {
                if (!members.TryGetValue(fieldId.Value, out var member))
                {
                    // Unknown field - skip value
                    reader.ReadValue(typeof(object));
                    continue;
                }

                Type memberType = member switch
                {
                    PropertyInfo pi => pi.PropertyType,
                    FieldInfo fi => fi.FieldType,
                    _ => typeof(object)
                };

                object? value = ReadValue(reader, memberType);
                if (member is PropertyInfo prop)
                {
                    prop.SetValue(instance, value);
                }
                else if (member is FieldInfo field)
                {
                    field.SetValue(instance, value);
                }
            }

            return instance;
        }

        private void WriteValue(INomadWriter writer, object? value, Type type)
        {
            var converter = GetConverter(type);
            if (converter is not null)
            {
                converter.Write(writer, value);
                return;
            }

            if (type.IsPrimitive || type == typeof(string) || type == typeof(byte[]))
            {
                writer.WriteValue(value, type);
                return;
            }

            WriteObject(writer, value, type);
        }

        private object? ReadValue(INomadReader reader, Type type)
        {
            var converter = GetConverter(type);
            if (converter is not null)
            {
                return converter.Read(reader, type);
            }

            if (type.IsPrimitive || type == typeof(string) || type == typeof(byte[]))
            {
                return reader.ReadValue(type);
            }

            return ReadObject(reader, type);
        }

        private INomadConverter? GetConverter(Type type)
        {
            foreach (var converter in _options.Converters)
            {
                if (converter.CanConvert(type))
                {
                    return converter;
                }
            }

            return null;
        }

        private IEnumerable<MemberInfo> GetSerializableMembers(Type type)
        {
            return _options.TypeInfoResolver.GetSerializableMembers(type);
        }
    }
}
