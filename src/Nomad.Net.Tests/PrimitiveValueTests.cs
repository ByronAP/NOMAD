using System;
using System.IO;
using System.Text;
using Nomad.Net.Serialization;
using Xunit;

namespace Nomad.Net.Tests
{
    /// <summary>
    /// Tests primitive value serialization.
    /// </summary>
    public sealed class PrimitiveValueTests
    {
        /// <summary>
        /// Validates round-trip serialization of <see cref="int"/> values.
        /// </summary>
        /// <param name="value">The value to serialize.</param>
        [Theory]
        [InlineData(123)]
        [InlineData(-1)]
        public void RoundTrip_Int32(int value)
        {
            using var ms = new MemoryStream();
            using (var writer = new NomadBinaryWriter(ms))
            {
                writer.WriteValue(value, typeof(int));
            }

            ms.Position = 0;
            using var reader = new NomadBinaryReader(ms);
            var result = reader.ReadValue(typeof(int));
            Assert.Equal(value, result);
        }

        /// <summary>
        /// Validates round-trip serialization of <see cref="long"/> values.
        /// </summary>
        /// <param name="value">The value to serialize.</param>
        [Theory]
        [InlineData(123L)]
        [InlineData(-5L)]
        public void RoundTrip_Int64(long value)
        {
            using var ms = new MemoryStream();
            using (var writer = new NomadBinaryWriter(ms))
            {
                writer.WriteValue(value, typeof(long));
            }

            ms.Position = 0;
            using var reader = new NomadBinaryReader(ms);
            var result = reader.ReadValue(typeof(long));
            Assert.Equal(value, result);
        }

        /// <summary>
        /// Validates round-trip serialization of <see cref="float"/> values.
        /// </summary>
        /// <param name="value">The value to serialize.</param>
        [Theory]
        [InlineData(1.5f)]
        [InlineData(-3.2f)]
        public void RoundTrip_Single(float value)
        {
            using var ms = new MemoryStream();
            using (var writer = new NomadBinaryWriter(ms))
            {
                writer.WriteValue(value, typeof(float));
            }

            ms.Position = 0;
            using var reader = new NomadBinaryReader(ms);
            var result = reader.ReadValue(typeof(float));
            Assert.Equal(value, result);
        }

        /// <summary>
        /// Validates round-trip serialization of <see cref="double"/> values.
        /// </summary>
        /// <param name="value">The value to serialize.</param>
        [Theory]
        [InlineData(2.5)]
        [InlineData(-8.1)]
        public void RoundTrip_Double(double value)
        {
            using var ms = new MemoryStream();
            using (var writer = new NomadBinaryWriter(ms))
            {
                writer.WriteValue(value, typeof(double));
            }

            ms.Position = 0;
            using var reader = new NomadBinaryReader(ms);
            var result = reader.ReadValue(typeof(double));
            Assert.Equal(value, result);
        }

        /// <summary>
        /// Validates round-trip serialization of <see cref="decimal"/> values.
        /// </summary>
        /// <param name="value">The value to serialize.</param>
        [Theory]
        [InlineData(1.5)]
        [InlineData(-3.25)]
        public void RoundTrip_Decimal(decimal value)
        {
            using var ms = new MemoryStream();
            using (var writer = new NomadBinaryWriter(ms))
            {
                writer.WriteValue(value, typeof(decimal));
            }

            ms.Position = 0;
            using var reader = new NomadBinaryReader(ms);
            var result = reader.ReadValue(typeof(decimal));
            Assert.Equal(value, result);
        }

        /// <summary>
        /// Validates round-trip serialization of <see cref="char"/> values.
        /// </summary>
        /// <param name="value">The value to serialize.</param>
        [Theory]
        [InlineData('A')]
        [InlineData('Z')]
        public void RoundTrip_Char(char value)
        {
            using var ms = new MemoryStream();
            using (var writer = new NomadBinaryWriter(ms))
            {
                writer.WriteValue(value, typeof(char));
            }

            ms.Position = 0;
            using var reader = new NomadBinaryReader(ms);
            var result = reader.ReadValue(typeof(char));
            Assert.Equal(value, result);
        }

        /// <summary>
        /// Validates round-trip serialization of <see cref="Rune"/> values.
        /// </summary>
        /// <param name="ch">The value to serialize.</param>
        [Theory]
        [InlineData('a')]
        [InlineData('\u2603')]
        public void RoundTrip_Rune(char ch)
        {
            var rune = new Rune(ch);
            using var ms = new MemoryStream();
            using (var writer = new NomadBinaryWriter(ms))
            {
                writer.WriteValue(rune, typeof(Rune));
            }

            ms.Position = 0;
            using var reader = new NomadBinaryReader(ms);
            var result = reader.ReadValue(typeof(Rune));
            Assert.Equal(rune, result);
        }

        /// <summary>
        /// Validates round-trip serialization of <see cref="bool"/> values.
        /// </summary>
        /// <param name="value">The value to serialize.</param>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void RoundTrip_Boolean(bool value)
        {
            using var ms = new MemoryStream();
            using (var writer = new NomadBinaryWriter(ms))
            {
                writer.WriteValue(value, typeof(bool));
            }

            ms.Position = 0;
            using var reader = new NomadBinaryReader(ms);
            var result = reader.ReadValue(typeof(bool));
            Assert.Equal(value, result);
        }

        /// <summary>
        /// Validates round-trip serialization of <see cref="string"/> values.
        /// </summary>
        /// <param name="value">The value to serialize.</param>
        [Theory]
        [InlineData("hello")]
        [InlineData("world")]
        public void RoundTrip_String(string value)
        {
            using var ms = new MemoryStream();
            using (var writer = new NomadBinaryWriter(ms))
            {
                writer.WriteValue(value, typeof(string));
            }

            ms.Position = 0;
            using var reader = new NomadBinaryReader(ms);
            var result = reader.ReadValue(typeof(string));
            Assert.Equal(value, result);
        }

        /// <summary>
        /// Validates round-trip serialization of binary data.
        /// </summary>
        [Fact]
        public void RoundTrip_Binary()
        {
            byte[] data = { 1, 2, 3, 4 };
            using var ms = new MemoryStream();
            using (var writer = new NomadBinaryWriter(ms))
            {
                writer.WriteValue(data, typeof(byte[]));
            }

            ms.Position = 0;
            using var reader = new NomadBinaryReader(ms);
            var result = (byte[])reader.ReadValue(typeof(byte[]))!;
            Assert.Equal(data, result);
        }

        /// <summary>
        /// Validates round-trip serialization of <see langword="null"/> values.
        /// </summary>
        [Fact]
        public void RoundTrip_Null()
        {
            using var ms = new MemoryStream();
            using (var writer = new NomadBinaryWriter(ms))
            {
                writer.WriteValue(null, typeof(string));
            }

            ms.Position = 0;
            using var reader = new NomadBinaryReader(ms);
            var result = reader.ReadValue(typeof(string));
            Assert.Null(result);
        }

        /// <summary>
        /// Validates round-trip serialization of nullable <see cref="int"/> values.
        /// </summary>
        /// <param name="value">The value to serialize.</param>
        [Theory]
        [InlineData(10)]
        [InlineData(null)]
        public void RoundTrip_NullableInt32(int? value)
        {
            using var ms = new MemoryStream();
            using (var writer = new NomadBinaryWriter(ms))
            {
                writer.WriteValue(value, typeof(int?));
            }

            ms.Position = 0;
            using var reader = new NomadBinaryReader(ms);
            var result = reader.ReadValue(typeof(int?));
            Assert.Equal(value, result);
        }

        /// <summary>
        /// Validates round-trip serialization of nullable <see cref="bool"/> values.
        /// </summary>
        /// <param name="value">The value to serialize.</param>
        [Theory]
        [InlineData(true)]
        [InlineData(null)]
        [InlineData(false)]
        public void RoundTrip_NullableBoolean(bool? value)
        {
            using var ms = new MemoryStream();
            using (var writer = new NomadBinaryWriter(ms))
            {
                writer.WriteValue(value, typeof(bool?));
            }

            ms.Position = 0;
            using var reader = new NomadBinaryReader(ms);
            var result = reader.ReadValue(typeof(bool?));
            Assert.Equal(value, result);
        }
    }
}
