using System.Reflection;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
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

            writer.WriteToken(NomadToken.StartObject);

            bool first = true;
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

                if (!first)
                {
                    writer.WriteToken(NomadToken.ValueSeparator);
                }

                int fieldId = fieldAttr?.FieldId ?? member.MetadataToken;
                writer.WriteFieldHeader(fieldId);
                writer.WriteToken(NomadToken.NameSeparator);
                WriteValue(writer, memberValue, memberValue.GetType());
                first = false;
            }

            writer.WriteToken(NomadToken.EndObject);
        }

        private object? ReadObject(INomadReader reader, Type type)
        {
            if (reader.ReadToken() != NomadToken.StartObject)
            {
                throw new FormatException("Expected start of object.");
            }

            object? instance = Activator.CreateInstance(type);
            var members = GetSerializableMembers(type).ToDictionary(m => m.GetCustomAttribute<NomadFieldAttribute>()?.FieldId ?? m.MetadataToken);

            if (reader.PeekToken() == NomadToken.EndObject)
            {
                reader.ReadToken();
                return instance;
            }

            while (true)
            {
                int? fieldId = reader.ReadFieldHeader();
                if (fieldId is null)
                {
                    throw new FormatException("Unexpected end of object.");
                }

                if (reader.ReadToken() != NomadToken.NameSeparator)
                {
                    throw new FormatException("Expected name separator.");
                }

                if (!members.TryGetValue(fieldId.Value, out var member))
                {
                    // Unknown field - skip value and signal unsupported
                    ReadValue(reader, typeof(object));
                    throw new NotSupportedException("Unknown field encountered");
                }
                else
                {
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

                var token = reader.ReadToken();
                if (token == NomadToken.EndObject)
                {
                    break;
                }

                if (token != NomadToken.ValueSeparator)
                {
                    throw new FormatException("Invalid object delimiter.");
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

            if (typeof(IDictionary).IsAssignableFrom(type) ||
                type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>)))
            {
                WriteMap(writer, value as IEnumerable, type);
                return;
            }

            if (typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string) && type != typeof(byte[]))
            {
                WriteArray(writer, (IEnumerable?)value, type);
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

            if (type == typeof(object))
            {
                NomadToken token = reader.PeekToken();
                if (token == NomadToken.StartObject)
                {
                    return ReadUntypedObject(reader);
                }
                else if (token == NomadToken.StartArray)
                {
                    return ReadUntypedArray(reader);
                }

                return reader.ReadValue(typeof(object));
            }

            if (type.IsPrimitive || type == typeof(string) || type == typeof(byte[]))
            {
                return reader.ReadValue(type);
            }

            if (typeof(IDictionary).IsAssignableFrom(type) ||
                type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>)))
            {
                return ReadMap(reader, type);
            }

            if (typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string) && type != typeof(byte[]))
            {
                return ReadArray(reader, type);
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

        /// <summary>
        /// Writes an enumerable to the output stream.
        /// </summary>
        /// <param name="writer">The writer instance.</param>
        /// <param name="values">The values to serialize.</param>
        /// <param name="type">The runtime type of the enumerable.</param>
        private void WriteArray(INomadWriter writer, IEnumerable? values, Type type)
        {
            Type elementType = type.IsArray ? type.GetElementType()! :
                type.IsGenericType ? type.GetGenericArguments()[0] : typeof(object);
            var list = values is ICollection col ?
                new List<object?>(col.Cast<object?>()) :
                values?.Cast<object?>().ToList() ?? new List<object?>();

            writer.WriteToken(NomadToken.StartArray);
            for (int i = 0; i < list.Count; i++)
            {
                if (i > 0)
                {
                    writer.WriteToken(NomadToken.ValueSeparator);
                }

                WriteValue(writer, list[i], elementType);
            }

            writer.WriteToken(NomadToken.EndArray);
        }

        /// <summary>
        /// Reads an enumerable value from the input stream.
        /// </summary>
        /// <param name="reader">The reader to consume.</param>
        /// <param name="type">The target enumerable type.</param>
        /// <returns>The populated enumerable instance.</returns>
        private object? ReadArray(INomadReader reader, Type type)
        {
            if (reader.ReadToken() != NomadToken.StartArray)
            {
                throw new FormatException("Expected start of array.");
            }

            Type elementType = type.IsArray ? type.GetElementType()! :
                type.IsGenericType ? type.GetGenericArguments()[0] : typeof(object);
            var items = new List<object?>();

            if (reader.PeekToken() == NomadToken.EndArray)
            {
                reader.ReadToken();
            }
            else
            {
                while (true)
                {
                    items.Add(ReadValue(reader, elementType));
                    var token = reader.ReadToken();
                    if (token == NomadToken.EndArray)
                    {
                        break;
                    }
                    if (token != NomadToken.ValueSeparator)
                    {
                        throw new FormatException("Invalid array delimiter.");
                    }
                }
            }

            if (type.IsArray)
            {
                Array array = Array.CreateInstance(elementType, items.Count);
                for (int i = 0; i < items.Count; i++)
                {
                    array.SetValue(items[i], i);
                }

                return array;
            }

            if (typeof(IList).IsAssignableFrom(type))
            {
                var result = (IList)Activator.CreateInstance(type)!;
                foreach (var item in items)
                {
                    result.Add(item);
                }

                return result;
            }

            var genericList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType))!;
            foreach (var item in items)
            {
                genericList.Add(item);
            }

            return genericList;
        }

        /// <summary>
        /// Writes a dictionary to the output stream.
        /// </summary>
        /// <param name="writer">The writer instance.</param>
        /// <param name="dictionary">The dictionary to serialize.</param>
        /// <param name="type">The runtime type of the dictionary.</param>
        private void WriteMap(INomadWriter writer, IEnumerable? dictionary, Type type)
        {
            Type keyType = typeof(object);
            Type valueType = typeof(object);
            if (type.IsGenericType)
            {
                var args = type.GetGenericArguments();
                keyType = args[0];
                valueType = args[1];
            }

            writer.WriteToken(NomadToken.StartObject);
            bool first = true;
            if (dictionary is not null)
            {
                foreach (var entry in dictionary)
                {
                    object? key;
                    object? value;
                    if (entry is DictionaryEntry de)
                    {
                        key = de.Key;
                        value = de.Value;
                    }
                    else
                    {
                        var eType = entry.GetType();
                        key = eType.GetProperty("Key")?.GetValue(entry);
                        value = eType.GetProperty("Value")?.GetValue(entry);
                    }

                    if (!first)
                    {
                        writer.WriteToken(NomadToken.ValueSeparator);
                    }

                    WriteValue(writer, key, keyType);
                    writer.WriteToken(NomadToken.NameSeparator);
                    WriteValue(writer, value, valueType);
                    first = false;
                }
            }

            writer.WriteToken(NomadToken.EndObject);
        }

        /// <summary>
        /// Reads a dictionary from the input stream.
        /// </summary>
        /// <param name="reader">The reader instance.</param>
        /// <param name="type">The runtime dictionary type.</param>
        /// <returns>The populated dictionary instance.</returns>
        private object? ReadMap(INomadReader reader, Type type)
        {
            if (reader.ReadToken() != NomadToken.StartObject)
            {
                throw new FormatException("Expected start of map.");
            }

            Type keyType = typeof(object);
            Type valueType = typeof(object);
            if (type.IsGenericType)
            {
                var args = type.GetGenericArguments();
                keyType = args[0];
                valueType = args[1];
            }

            var entries = new List<(object? Key, object? Value)>();

            if (reader.PeekToken() == NomadToken.EndObject)
            {
                reader.ReadToken();
            }
            else
            {
                while (true)
                {
                    object? key = ReadValue(reader, keyType);
                    if (reader.ReadToken() != NomadToken.NameSeparator)
                    {
                        throw new FormatException("Expected name separator.");
                    }

                    object? value = ReadValue(reader, valueType);
                    entries.Add((key, value));

                    var token = reader.ReadToken();
                    if (token == NomadToken.EndObject)
                    {
                        break;
                    }
                    if (token != NomadToken.ValueSeparator)
                    {
                        throw new FormatException("Invalid map delimiter.");
                    }
                }
            }

            IDictionary result;
            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                result = (IDictionary)Activator.CreateInstance(type)!;
            }
            else
            {
                var dictType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
                result = (IDictionary)Activator.CreateInstance(dictType)!;
            }

            foreach (var (Key, Value) in entries)
            {
                result.Add(Key, Value);
            }

            return result;
        }

        private Dictionary<int, object?> ReadUntypedObject(INomadReader reader)
        {
            if (reader.ReadToken() != NomadToken.StartObject)
            {
                throw new FormatException("Expected start of object.");
            }

            var result = new Dictionary<int, object?>();
            if (reader.PeekToken() == NomadToken.EndObject)
            {
                reader.ReadToken();
                return result;
            }

            while (true)
            {
                int? fieldId = reader.ReadFieldHeader();
                if (fieldId is null)
                {
                    throw new FormatException("Unexpected end of object.");
                }

                if (reader.ReadToken() != NomadToken.NameSeparator)
                {
                    throw new FormatException("Expected name separator.");
                }

                result[fieldId.Value] = ReadValue(reader, typeof(object));

                var token = reader.ReadToken();
                if (token == NomadToken.EndObject)
                {
                    break;
                }
                if (token != NomadToken.ValueSeparator)
                {
                    throw new FormatException("Invalid object delimiter.");
                }
            }

            return result;
        }

        private List<object?> ReadUntypedArray(INomadReader reader)
        {
            if (reader.ReadToken() != NomadToken.StartArray)
            {
                throw new FormatException("Expected start of array.");
            }

            var items = new List<object?>();

            if (reader.PeekToken() == NomadToken.EndArray)
            {
                reader.ReadToken();
            }
            else
            {
                while (true)
                {
                    items.Add(ReadValue(reader, typeof(object)));
                    var token = reader.ReadToken();
                    if (token == NomadToken.EndArray)
                    {
                        break;
                    }
                    if (token != NomadToken.ValueSeparator)
                    {
                        throw new FormatException("Invalid array delimiter.");
                    }
                }
            }

            return items;
        }

        private IEnumerable<MemberInfo> GetSerializableMembers(Type type)
        {
            var resolverAttr = type.GetCustomAttribute<Attributes.NomadResolverAttribute>();

            if (resolverAttr is not null &&
                Type.GetType(resolverAttr.ResolverType) is { } resolverType &&
                Activator.CreateInstance(resolverType) is INomadTypeInfoResolver resolver)
            {
                return resolver.GetSerializableMembers(type);
            }

            return _options.TypeInfoResolver.GetSerializableMembers(type);
        }
    }
}
