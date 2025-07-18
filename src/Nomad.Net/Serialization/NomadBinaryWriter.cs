using System.Text;

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
        public void WriteValue(object? value, Type type)
        {
            if (value is null)
            {
                _writer.Write((byte)0);
                return;
            }

            if (type == typeof(int))
            {
                _writer.Write((byte)1);
                _writer.Write((int)value);
            }
            else if (type == typeof(string))
            {
                _writer.Write((byte)2);
                _writer.Write((string)value);
            }
            else if (type == typeof(byte[]))
            {
                _writer.Write((byte)3);
                var buffer = (byte[])value;
                _writer.Write(buffer.Length);
                _writer.Write(buffer);
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
