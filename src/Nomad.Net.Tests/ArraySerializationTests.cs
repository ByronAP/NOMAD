using System.IO;
using Nomad.Net.Serialization;
using Nomad.Net.Tests.Models;
using Xunit;

namespace Nomad.Net.Tests
{
    /// <summary>
    /// Tests array serialization semantics.
    /// </summary>
    public sealed class ArraySerializationTests
    {
        /// <summary>
        /// Ensures arrays of integers round-trip correctly.
        /// </summary>
        [Fact]
        public void RoundTrip_IntArray()
        {
            var container = new IntArrayContainer { Values = new[] { 1, 2, 3 } };
            var serializer = new NomadSerializer();
            using var ms = new MemoryStream();
            using (var writer = new NomadBinaryWriter(ms))
            {
                serializer.Serialize(writer, container);
            }

            ms.Position = 0;
            using var reader = new NomadBinaryReader(ms);
            var result = serializer.Deserialize<IntArrayContainer>(reader);
            Assert.Equal(container.Values, result!.Values);
        }

        /// <summary>
        /// Ensures arrays of strings round-trip correctly.
        /// </summary>
        [Fact]
        public void RoundTrip_StringList()
        {
            var container = new StringArrayContainer { Values = new[] { "a", "b" } };
            var serializer = new NomadSerializer();
            using var ms = new MemoryStream();
            using (var writer = new NomadBinaryWriter(ms))
            {
                serializer.Serialize(writer, container);
            }

            ms.Position = 0;
            using var reader = new NomadBinaryReader(ms);
            var result = serializer.Deserialize<StringArrayContainer>(reader);
            Assert.Equal(container.Values, result!.Values);
        }
    }
}
