using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nomad.Net.Serialization;
using Nomad.Net.Tests.Models;
using Xunit;

namespace Nomad.Net.Tests
{
    /// <summary>
    /// Tests serialization when the declared type is <see cref="object"/>.
    /// </summary>
    public sealed class DynamicValueTests
    {
        /// <summary>
        /// Verifies round-trip of a primitive value stored as <see cref="object"/>.
        /// </summary>
        [Fact]
        public void RoundTrip_ObjectPrimitive()
        {
            var serializer = new NomadSerializer();
            var container = new ObjectContainer { Value = 5 };
            using var ms = new MemoryStream();
            using (var writer = new NomadBinaryWriter(ms))
            {
                serializer.Serialize(writer, container);
            }

            ms.Position = 0;
            using var reader = new NomadBinaryReader(ms);
            var result = serializer.Deserialize<ObjectContainer>(reader);
            Assert.Equal(5, result!.Value);
        }

        /// <summary>
        /// Verifies round-trip of an array stored as <see cref="object"/>.
        /// </summary>
        [Fact]
        public void RoundTrip_ObjectArray()
        {
            var serializer = new NomadSerializer();
            var container = new ObjectContainer { Value = new[] { 1, 2 } };
            using var ms = new MemoryStream();
            using (var writer = new NomadBinaryWriter(ms))
            {
                serializer.Serialize(writer, container);
            }

            ms.Position = 0;
            using var reader = new NomadBinaryReader(ms);
            var result = serializer.Deserialize<ObjectContainer>(reader);
            var list = Assert.IsType<List<object?>>(result!.Value);
            Assert.Equal(new[] { 1, 2 }, list.Cast<int>().ToArray());
        }
    }
}
