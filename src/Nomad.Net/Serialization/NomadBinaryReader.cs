using System;
using System.IO;
using System.Text;

namespace Nomad.Net.Serialization
{
    /// <summary>
    /// Default implementation of <see cref="INomadReader"/> that reads the NOMAD format from a <see cref="Stream"/>.
    /// </summary>
    public sealed class NomadBinaryReader : INomadReader, IDisposable
    {
        private readonly BinaryReader _reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="NomadBinaryReader"/> class.
        /// </summary>
        /// <param name="stream">The input stream.</param>
        public NomadBinaryReader(Stream stream)
        {
            _reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);
        }

        /// <inheritdoc />
        public int? ReadFieldHeader()
        {
            if (_reader.BaseStream.Position == _reader.BaseStream.Length)
            {
                return null;
            }

            return _reader.Read7BitEncodedInt();
        }

        /// <inheritdoc />
        public object? ReadValue(Type type)
        {
            byte kind = _reader.ReadByte();
            if (kind == 0)
            {
                return null;
            }

            if (type == typeof(int) && kind == 1)
            {
                return _reader.ReadInt32();
            }
            else if (type == typeof(string) && kind == 2)
            {
                return _reader.ReadString();
            }
            else if (type == typeof(byte[]) && kind == 3)
            {
                int len = _reader.ReadInt32();
                return _reader.ReadBytes(len);
            }

            throw new NotSupportedException($"Unsupported type: {type}");
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _reader.Dispose();
        }
    }
}
