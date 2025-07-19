using System;
using System.IO;
using Nomad.Net.Serialization;
using Xunit;

namespace Nomad.Net.Tests
{
    /// <summary>
    /// Tests for varint decoding limits.
    /// </summary>
    public sealed class VarintDecodingTests
    {
        /// <summary>
        /// Ensures the reader rejects field headers exceeding 10 bytes.
        /// </summary>
        [Fact]
        public void FieldHeader_TooLong_Throws()
        {
            byte[] data = new byte[11];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = 0x80; // continuation bits set
            }

            using var ms = new MemoryStream(data);
            using var reader = new NomadBinaryReader(ms);
            Assert.Throws<FormatException>(() => reader.ReadFieldHeader());
        }
    }
}
