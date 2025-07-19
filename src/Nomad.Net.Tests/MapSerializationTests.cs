using System.Collections.Generic;
using System.IO;
using Nomad.Net.Serialization;
using Nomad.Net.Tests.Models;
using Xunit;

namespace Nomad.Net.Tests
{
    /// <summary>
    /// Tests dictionary serialization semantics.
    /// </summary>
    public sealed class MapSerializationTests
    {
        /// <summary>
        /// Ensures dictionaries round-trip correctly.
        /// </summary>
        [Fact]
        public void RoundTrip_Dictionary()
        {
            var container = new DictionaryContainer
            {
                Values = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 }
            };
            var serializer = new NomadSerializer();
            using var ms = new MemoryStream();
            using (var writer = new NomadBinaryWriter(ms))
            {
                serializer.Serialize(writer, container);
            }

            ms.Position = 0;
            using var reader = new NomadBinaryReader(ms);
            var result = serializer.Deserialize<DictionaryContainer>(reader);
            Assert.Equal(container.Values, result!.Values);
        }

        /// <summary>
        /// Ensures empty dictionaries are supported.
        /// </summary>
        [Fact]
        public void RoundTrip_EmptyDictionary()
        {
            var container = new DictionaryContainer { Values = new Dictionary<string, int>() };
            var serializer = new NomadSerializer();
            using var ms = new MemoryStream();
            using (var writer = new NomadBinaryWriter(ms))
            {
                serializer.Serialize(writer, container);
            }

            ms.Position = 0;
            using var reader = new NomadBinaryReader(ms);
            var result = serializer.Deserialize<DictionaryContainer>(reader);
            Assert.Empty(result!.Values!);
        }
    }
}
