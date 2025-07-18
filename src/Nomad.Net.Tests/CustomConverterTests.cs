using System;
using System.IO;
using Nomad.Net.Serialization;
using Nomad.Net.Tests.Models;
using Xunit;

namespace Nomad.Net.Tests
{
    /// <summary>
    /// Tests custom converter usage.
    /// </summary>
    public sealed class CustomConverterTests
    {
        /// <summary>
        /// Exercises a custom converter for <see cref="Guid"/> values.
        /// </summary>
        [Fact]
        public void GuidConverter_RoundTrip()
        {
            var options = new NomadSerializerOptions();
            options.Converters.Add(new GuidConverter());
            var serializer = new NomadSerializer(options);
            var container = new GuidContainer { Id = Guid.NewGuid() };
            using var ms = new MemoryStream();
            using (var writer = new NomadBinaryWriter(ms))
            {
                serializer.Serialize(writer, container);
            }

            ms.Position = 0;
            using var reader = new NomadBinaryReader(ms);
            var result = serializer.Deserialize<GuidContainer>(reader);
            Assert.Equal(container.Id, result!.Id);
        }
    }
}
