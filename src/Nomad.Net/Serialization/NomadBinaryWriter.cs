using System.Text;
using System;

namespace Nomad.Net.Serialization
{
    /// <summary>
    /// Default implementation of <see cref="INomadWriter"/> that writes the NOMAD format to a <see cref="Stream"/>.
    /// </summary>
    public sealed class NomadBinaryWriter : INomadWriter, IDisposable
    {
        private readonly BinaryWriter _writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="NomadBinaryWriter"/> class.
        /// </summary>
        /// <param name="stream">The output stream.</param>
        public NomadBinaryWriter(Stream stream)
        {
            _writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true);
        }

        /// <inheritdoc />
        public void WriteFieldHeader(int fieldId)
        {
            _writer.Write7BitEncodedInt(fieldId);
        }

        /// <inheritdoc />
        public void WriteToken(NomadToken token)
        {
            _writer.Write((byte)token);
        }

        /// <inheritdoc />
        public void WriteValue(object? value, Type type)
        {
            if (value is null)
            {
                _writer.Write((byte)NomadValueKind.Null);
                return;
            }

            if (type == typeof(int))
            {
                _writer.Write((byte)NomadValueKind.Int32);
                _writer.Write((int)value);
            }
            else if (type == typeof(string))
            {
                _writer.Write((byte)NomadValueKind.String);
                _writer.Write((string)value);
            }
            else if (type == typeof(byte[]))
            {
                _writer.Write((byte)NomadValueKind.Binary);
                var buffer = (byte[])value;
                _writer.Write(buffer.Length);
                _writer.Write(buffer);
            }
            else if (type == typeof(bool))
            {
                _writer.Write((byte)NomadValueKind.Boolean);
                _writer.Write((bool)value ? (byte)1 : (byte)0);
            }
            else if (type == typeof(long))
            {
                _writer.Write((byte)NomadValueKind.Int64);
                _writer.Write((long)value);
            }
            else if (type == typeof(float))
            {
                _writer.Write((byte)NomadValueKind.Single);
                _writer.Write((float)value);
            }
            else if (type == typeof(double))
            {
                _writer.Write((byte)NomadValueKind.Double);
                _writer.Write((double)value);
            }
            else
            {
                throw new NotSupportedException($"Unsupported type: {type}");
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _writer.Dispose();
        }
    }
}
