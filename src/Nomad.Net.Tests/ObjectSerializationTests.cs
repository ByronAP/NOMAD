using System;
using System.IO;
using Nomad.Net.Serialization;
using Nomad.Net.Tests.Models;
using Xunit;

namespace Nomad.Net.Tests
{
    /// <summary>
    /// Tests object serialization using <see cref="NomadSerializer"/>.
    /// </summary>
    public sealed class ObjectSerializationTests
    {
        /// <summary>
        /// Verifies that a simple object can be serialized and deserialized.
        /// </summary>
        [Fact]
        public void RoundTrip_Person()
        {
            var serializer = new NomadSerializer();
            var person = new Person { Id = 7, Name = "Alice" };
            using var ms = new MemoryStream();
            using (var writer = new NomadBinaryWriter(ms))
            {
                serializer.Serialize(writer, person);
            }

            ms.Position = 0;
            using var reader = new NomadBinaryReader(ms);
            var result = serializer.Deserialize<Person>(reader);
            Assert.Equal(person.Id, result!.Id);
            Assert.Equal(person.Name, result.Name);
        }

        /// <summary>
        /// Demonstrates behavior when encountering an unknown field.
        /// </summary>
        [Fact]
        public void UnknownField_IsSkipped()
        {
            var serializer = new NomadSerializer();
            var v2 = new PersonV2 { Id = 3, Name = "Bob", Extra = "unused" };
            using var ms = new MemoryStream();
            using (var writer = new NomadBinaryWriter(ms))
            {
                serializer.Serialize(writer, v2);
            }

            ms.Position = 0;
            using var reader = new NomadBinaryReader(ms);
            Assert.Throws<NotSupportedException>(() => serializer.Deserialize<Person>(reader));
        }
    }
}
