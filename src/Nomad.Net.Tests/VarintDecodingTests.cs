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

        /// <summary>
        /// Validates the boundary conditions for field header length.
        /// </summary>
        [Fact]
        public void FieldHeader_Boundaries()
        {
            // Ten bytes with nine continuation bits should succeed.
            byte[] valid = new byte[10];
            for (int i = 0; i < valid.Length - 1; i++)
            {
                valid[i] = 0x80;
            }

            valid[valid.Length - 1] = 0x00;

            using (var ms = new MemoryStream(valid))
            using (var reader = new NomadBinaryReader(ms))
            {
                int? fieldId = reader.ReadFieldHeader();
                Assert.NotNull(fieldId);
            }

            // Eleven bytes should throw a format exception.
            byte[] invalid = new byte[11];
            for (int i = 0; i < invalid.Length; i++)
            {
                invalid[i] = 0x80;
            }

            using var ms2 = new MemoryStream(invalid);
            using var reader2 = new NomadBinaryReader(ms2);
            Assert.Throws<FormatException>(() => reader2.ReadFieldHeader());
        }
    }
}
