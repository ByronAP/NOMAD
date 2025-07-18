using System.Linq;
using System.Reflection;
using Nomad.Net.Attributes;
using Nomad.Net.Serialization;
using Nomad.Net.Tests.Models;
using Xunit;

namespace Nomad.Net.Tests
{
    /// <summary>
    /// Tests behavior of <see cref="ReflectionNomadTypeInfoResolver"/>.
    /// </summary>
    public sealed class ReflectionResolverTests
    {
        private sealed class Sample
        {
            [NomadField(1)]
            public int Included { get; set; }

            [NomadIgnore]
            public int Ignored { get; set; }
        }

        /// <summary>
        /// Ensures the resolver filters ignored members.
        /// </summary>
        [Fact]
        public void Resolver_ReturnsExpectedMembers()
        {
            var resolver = new ReflectionNomadTypeInfoResolver();
            var members = resolver.GetSerializableMembers(typeof(Sample));
            Assert.Contains(members, m => m.Name == nameof(Sample.Included));
            Assert.DoesNotContain(members, m => m.Name == nameof(Sample.Ignored));
        }
    }
}
