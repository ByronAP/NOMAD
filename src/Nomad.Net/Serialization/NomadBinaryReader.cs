using System.Text;
using System;

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
            if (kind == (byte)NomadValueKind.Null)
            {
                return null;
            }

            if (type == typeof(int) && kind == (byte)NomadValueKind.Int32)
            {
                return _reader.ReadInt32();
            }
            else if (type == typeof(string) && kind == (byte)NomadValueKind.String)
            {
                return _reader.ReadString();
            }
            else if (type == typeof(byte[]) && kind == (byte)NomadValueKind.Binary)
            {
                int len = _reader.ReadInt32();
                return _reader.ReadBytes(len);
            }
            else if (type == typeof(bool) && kind == (byte)NomadValueKind.Boolean)
            {
                return _reader.ReadByte() != 0;
            }
            else if (type == typeof(long) && kind == (byte)NomadValueKind.Int64)
            {
                return _reader.ReadInt64();
            }
            else if (type == typeof(float) && kind == (byte)NomadValueKind.Single)
            {
                return _reader.ReadSingle();
            }
            else if (type == typeof(double) && kind == (byte)NomadValueKind.Double)
            {
                return _reader.ReadDouble();
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
