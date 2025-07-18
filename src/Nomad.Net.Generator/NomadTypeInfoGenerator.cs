using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Nomad.Net.Generator
{
    /// <summary>
    /// Generates a compile-time resolver used for ahead-of-time serialization.
    /// </summary>
    [Generator]
    public sealed class NomadTypeInfoGenerator : ISourceGenerator
    {
        /// <inheritdoc />
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        /// <inheritdoc />
        public void Execute(GeneratorExecutionContext context)
        {
            var compilation = context.Compilation;
            var fields = new Dictionary<INamedTypeSymbol, List<IFieldSymbol>>();
            var properties = new Dictionary<INamedTypeSymbol, List<IPropertySymbol>>();

            foreach (var tree in compilation.SyntaxTrees)
            {
                var model = compilation.GetSemanticModel(tree);
                foreach (var typeSyntax in tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>())
                {
                    if (model.GetDeclaredSymbol(typeSyntax) is not INamedTypeSymbol typeSymbol)
                    {
                        continue;
                    }

                    bool custom = typeSymbol.GetAttributes().Any(a =>
                        a.AttributeClass?.Name == "NomadResolverAttribute");
                    if (custom)
                    {
                        continue;
                    }

                    foreach (var member in typeSymbol.GetMembers())
                    {
                        if (member is IFieldSymbol field && !field.GetAttributes().Any(a => a.AttributeClass?.Name == "NomadIgnoreAttribute"))
                        {
                            if (!fields.ContainsKey(typeSymbol))
                            {
                                fields[typeSymbol] = new List<IFieldSymbol>();
                            }

                            fields[typeSymbol].Add(field);
                        }
                        else if (member is IPropertySymbol prop && !prop.GetAttributes().Any(a => a.AttributeClass?.Name == "NomadIgnoreAttribute"))
                        {
                            if (!properties.ContainsKey(typeSymbol))
                            {
                                properties[typeSymbol] = new List<IPropertySymbol>();
                            }

                            properties[typeSymbol].Add(prop);
                        }
                    }
                }
            }

            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Reflection;");
            sb.AppendLine("namespace Nomad.Net.Serialization");
            sb.AppendLine("{");
            sb.AppendLine("    /// <summary>");
            sb.AppendLine("    /// Resolver generated at compile time.");
            sb.AppendLine("    /// </summary>");
            sb.AppendLine("    internal sealed class GeneratedNomadTypeInfoResolver : INomadTypeInfoResolver");
            sb.AppendLine("    {");
            sb.AppendLine("        private static readonly Dictionary<Type, MemberInfo[]> Map = new()");
            sb.AppendLine("        {");
            foreach (var kvp in fields)
            {
                sb.Append("            { typeof(");
                sb.Append(kvp.Key.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
                sb.AppendLine("), new MemberInfo[] {");
                foreach (var f in kvp.Value)
                {
                    sb.Append("                typeof(");
                    sb.Append(kvp.Key.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
                    sb.Append(").GetField(\"");
                    sb.Append(f.Name);
                    sb.AppendLine("\")!,");
                }
                if (properties.TryGetValue(kvp.Key, out var props))
                {
                    foreach (var p in props)
                    {
                        sb.Append("                typeof(");
                        sb.Append(kvp.Key.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
                        sb.Append(").GetProperty(\"");
                        sb.Append(p.Name);
                        sb.AppendLine("\")!,");
                    }
                }
                sb.AppendLine("            } },");
            }
            sb.AppendLine("        };\n");
            sb.AppendLine("        /// <inheritdoc />");
            sb.AppendLine("        public IReadOnlyList<MemberInfo> GetSerializableMembers(Type type)");
            sb.AppendLine("        {");
            sb.AppendLine("            return Map.TryGetValue(type, out var m) ? (IReadOnlyList<MemberInfo>)m : Array.Empty<MemberInfo>();");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            context.AddSource("GeneratedNomadTypeInfoResolver.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
        }
    }
}
