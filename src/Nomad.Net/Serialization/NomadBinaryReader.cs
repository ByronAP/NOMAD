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
        private bool _hasPeeked;
        private byte _peekByte;

        /// <summary>
        /// Initializes a new instance of the <see cref="NomadBinaryReader"/> class.
        /// </summary>
        /// <param name="stream">The input stream.</param>
        public NomadBinaryReader(Stream stream)
        {
            _reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);
        }

        private byte ReadByteInternal()
        {
            if (_hasPeeked)
            {
                _hasPeeked = false;
                return _peekByte;
            }

            return _reader.ReadByte();
        }

        /// <inheritdoc />
        public int? ReadFieldHeader()
        {
            if (!_hasPeeked && _reader.BaseStream.Position == _reader.BaseStream.Length)
            {
                return null;
            }

            int result = 0;
            int shift = 0;
            for (int i = 0; i < 10; i++)
            {
                byte b = ReadByteInternal();
                result |= (b & 0x7F) << shift;
                if ((b & 0x80) == 0)
                {
                    return result;
                }

                shift += 7;
            }

            throw new FormatException("Invalid field header encoding.");
        }

        /// <inheritdoc />
        public NomadToken ReadToken()
        {
            return (NomadToken)ReadByteInternal();
        }

        /// <inheritdoc />
        public NomadToken PeekToken()
        {
            if (!_hasPeeked)
            {
                _peekByte = _reader.ReadByte();
                _hasPeeked = true;
            }

            return (NomadToken)_peekByte;
        }

        /// <inheritdoc />
        public object? ReadValue(Type type)
        {
            byte kind = ReadByteInternal();
            if (kind == (byte)NomadValueKind.Null)
            {
                return null;
            }

            if (type == typeof(object))
            {
                return kind switch
                {
                    (byte)NomadValueKind.Int32 => _reader.ReadInt32(),
                    (byte)NomadValueKind.String => _reader.ReadString(),
                    (byte)NomadValueKind.Binary => ReadRemainingBinary(),
                    (byte)NomadValueKind.Boolean => ReadByteInternal() != 0,
                    (byte)NomadValueKind.Int64 => _reader.ReadInt64(),
                    (byte)NomadValueKind.Single => _reader.ReadSingle(),
                    (byte)NomadValueKind.Double => _reader.ReadDouble(),
                    (byte)NomadValueKind.Decimal => _reader.ReadDecimal(),
                    (byte)NomadValueKind.Char => (char)_reader.ReadByte(),
                    (byte)NomadValueKind.Rune => new Rune(_reader.ReadInt32()),
                    _ => throw new NotSupportedException($"Unsupported value kind: {kind}")
                };
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
                return ReadRemainingBinary();
            }
            else if (type == typeof(bool) && kind == (byte)NomadValueKind.Boolean)
            {
                return ReadByteInternal() != 0;
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
            else if (type == typeof(decimal) && kind == (byte)NomadValueKind.Decimal)
            {
                return _reader.ReadDecimal();
            }
            else if (type == typeof(char) && kind == (byte)NomadValueKind.Char)
            {
                return (char)_reader.ReadByte();
            }
            else if (type == typeof(Rune) && kind == (byte)NomadValueKind.Rune)
            {
                return new Rune(_reader.ReadInt32());
            }

            throw new NotSupportedException($"Unsupported type: {type}");
        }

        private byte[] ReadRemainingBinary()
        {
            long remaining = _reader.BaseStream.Length - _reader.BaseStream.Position;
            return _reader.ReadBytes((int)remaining);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _reader.Dispose();
        }
    }
}
