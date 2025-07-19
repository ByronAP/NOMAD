using System;
using System.IO;
using Nomad.Net.Serialization;
using Nomad.Net.Tests.Models;
using Xunit;

namespace Nomad.Net.Tests
{
    /// <summary>
    /// Tests validation of field identifiers during serialization.
    /// </summary>
    public sealed class FieldIdValidationTests
    {
        /// <summary>
        /// Ensures duplicate field identifiers cause an exception.
        /// </summary>
        [Fact]
        public void DuplicateIds_Throws()
        {
            var serializer = new NomadSerializer();
            var model = new DuplicateFieldIdModel { Value1 = 1, Value2 = 2 };
            using var ms = new MemoryStream();
            using var writer = new NomadBinaryWriter(ms);
            Assert.Throws<InvalidOperationException>(() => serializer.Serialize(writer, model));
        }

        /// <summary>
        /// Ensures a field identifier of zero is rejected.
        /// </summary>
        [Fact]
        public void ZeroId_Throws()
        {
            var serializer = new NomadSerializer();
            var model = new ZeroFieldIdModel { Value = 5 };
            using var ms = new MemoryStream();
            using var writer = new NomadBinaryWriter(ms);
            Assert.Throws<InvalidOperationException>(() => serializer.Serialize(writer, model));
        }
    }
}
