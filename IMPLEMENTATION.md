# NOMAD .NET Library Specification

This document describes the architecture and implementation guidelines for the official NOMAD .NET library. The goal of this library is to provide a production ready serializer and deserializer while remaining fully extensible.

## Goals

- Easy to use defaults that work with plain objects.
- Extensibility through interfaces so advanced scenarios are possible.
- Optional attributes for precise control similar to `System.Text.Json`.
- Generated XML documentation to assist consumers and tooling.
- Compatibility with any CLR object even when no attributes are present.

## Project Structure

The library is implemented in the `src/Nomad.Net` folder. It targets **.NET 9.0** and produces XML documentation by default.

### Key Components

- `INomadWriter` / `INomadReader` – abstractions over the binary format.
- `NomadBinaryWriter` / `NomadBinaryReader` – default implementations based on `System.IO`.
- `INomadConverter` – extensibility point for custom serialization and deserialization.
- `NomadSerializerOptions` – configuration container including custom converters and policy settings.
- `NomadSerializer` – high level serializer that reflects over objects and uses the configured writer and reader.
- `INomadTypeInfoResolver` – abstraction that supplies serializable members for a type. The default implementation uses reflection but source generators can provide AOT friendly metadata.
- Attribute types under the `Nomad.Net.Attributes` namespace provide optional metadata:
  - `NomadFieldAttribute` – explicit field identifiers.
  - `NomadIgnoreAttribute` – skip a member.
  - `NomadMetaAttribute` – custom metadata similar to JSON annotations.

## Using the Library

1. **Serializing**

   ```csharp
   var serializer = new NomadSerializer();
   using var writer = new NomadBinaryWriter(stream);
   serializer.Serialize(writer, myObject);
   ```

2. **Deserializing**

   ```csharp
   using var reader = new NomadBinaryReader(stream);
   var result = serializer.Deserialize<MyType>(reader);
   ```

3. **Extending with Converters**

   Implement `INomadConverter` to handle special types or custom logic and register it in `NomadSerializerOptions.Converters`.

## Building

```
dotnet build src/Nomad.sln -c Release
```

The build produces `Nomad.Net.dll` and the XML documentation file in the project output directory. These can be packed into a NuGet package using `dotnet pack`.

## NuGet Packaging

```
dotnet pack src/Nomad.Net/Nomad.Net.csproj -c Release
```

## Style Guidelines

- Only one type declaration per file.
- All public members are documented with XML comments.
- Nullable reference types are enabled.
- `LangVersion` is set to `latest` to allow modern C# syntax.

## Ahead-Of-Time (AOT) Support

The serializer can operate without runtime reflection by providing an `INomadTypeInfoResolver`
implementation generated at compile time. This allows applications targeting
NativeAOT to supply precomputed metadata for their types. The
`ReflectionNomadTypeInfoResolver` remains the default for convenient dynamic
usage.

