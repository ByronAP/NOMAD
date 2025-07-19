using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Nomad.Net.Generator;
using Nomad.Net.Attributes;
using Xunit;

namespace Nomad.Net.Generator.Tests
{
    /// <summary>
    /// Tests for <see cref="NomadTypeInfoGenerator"/>.
    /// </summary>
    public sealed class NomadTypeInfoGeneratorTests
    {
        /// <summary>
        /// Verifies that structs without a custom resolver are included in the generated metadata.
        /// </summary>
        [Fact]
        public void Structs_AreProcessed()
        {
            const string source = """
using Nomad.Net.Attributes;

public struct SampleStruct
{
    [NomadField(1)]
    public int Value;
}

[NomadResolver("CustomResolver")]
public struct CustomStruct
{
    public int Ignored;
}
""";

            string? tpa = (string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES");
            var references = tpa!.Split(Path.PathSeparator)
                .Select(p => MetadataReference.CreateFromFile(p))
                .Concat(new[] { MetadataReference.CreateFromFile(typeof(NomadFieldAttribute).Assembly.Location) });

            var compilation = CSharpCompilation.Create(
                "Tests",
                new[] { CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Latest)) },
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var generator = new NomadTypeInfoGenerator();
            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
            driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
            using var stream = new MemoryStream();
            var result = outputCompilation.Emit(stream);
            Assert.True(result.Success, string.Join(Environment.NewLine, result.Diagnostics));

            stream.Position = 0;
            var assembly = Assembly.Load(stream.ToArray());
            var resolverType = assembly.GetType("Nomad.Net.Serialization.GeneratedNomadTypeInfoResolver")!;
            var resolver = Activator.CreateInstance(resolverType)!;
            var method = resolverType.GetMethod("GetSerializableMembers")!;
            var members = (IReadOnlyList<MemberInfo>)method.Invoke(resolver, new object?[] { assembly.GetType("SampleStruct")! })!;
            Assert.Contains(members, m => m.Name == "Value");

            var customMembers = (IReadOnlyList<MemberInfo>)method.Invoke(resolver, new object?[] { assembly.GetType("CustomStruct")! })!;
            Assert.Empty(customMembers);
        }
    }
}
