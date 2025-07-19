# NOMAD .NET Library Specification

This document describes the architecture and implementation guidelines for the official NOMAD .NET library. The goal of this library is to provide a production ready serializer and deserializer while remaining fully extensible.

## Goals

- Easy to use defaults that work with plain objects.
- Extensibility through interfaces so advanced scenarios are possible.
- Optional attributes for precise control similar to `System.Text.Json`.
- Generated XML documentation to assist consumers and tooling.
- Compatibility with any CLR object even when no attributes are present.
- Capable of ahead-of-time (AOT) compilation through the `INomadTypeInfoResolver` abstraction.

## Project Structure

The library is implemented in the `src/Nomad.Net` folder. It targets **.NET 9.0** and produces XML documentation by default.

### Key Components

- `INomadWriter` / `INomadReader` – abstractions over the binary format.
- `NomadBinaryWriter` / `NomadBinaryReader` – default implementations based on `System.IO`.
  Field headers are encoded as varints and both the writer and reader enforce the 10 byte maximum
  mandated by the specification.
- `INomadConverter` – extensibility point for custom serialization and deserialization.
- `NomadSerializerOptions` – configuration container including custom converters and policy settings.
- `NomadSerializer` – high level serializer that reflects over objects and uses the configured writer and reader.
- `INomadTypeInfoResolver` – abstraction that supplies serializable members for a type. The default implementation uses reflection but a source generator emits a `GeneratedNomadTypeInfoResolver` for AOT scenarios.
- `NomadValueKind` – enumeration of primitive markers used by the binary writer and reader.
- Dynamic values declared as `object` are supported. Primitive kinds are emitted with their
  `NomadValueKind` marker, while arrays and objects are materialized as generic collections.
  Supported kinds include integers, floating point numbers, decimals, booleans, strings,
  binary blobs, single-byte characters and Unicode runes.
- Nullable primitive types are detected via `Nullable.GetUnderlyingType` and serialized as
  their underlying kind. A `null` value is encoded using the `NomadValueKind.Null` marker.
- Binary data is written as raw bytes without a length prefix. Readers compute the length from the enclosing structure or fixed-size type information.
- Arrays and collections are supported natively and encoded using structural delimiters without length prefixes.
- Dictionaries are supported using the same structural delimiters with key/value pairs.
- Attribute types under the `Nomad.Net.Attributes` namespace provide optional metadata:
  - `NomadFieldAttribute` – explicit field identifiers.
  - `NomadIgnoreAttribute` – skip a member.
  - `NomadMetaAttribute` – custom metadata similar to JSON annotations.
  - `NomadResolverAttribute` – specify a custom `INomadTypeInfoResolver` for a type using `NomadResolver("My.Custom.Resolver")`.
  - Attributes apply equally to classes and structs so value types can participate in serialization.

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

The library **MUST** operate in environments where runtime reflection is not
available. All serialization metadata can be supplied at compile time via an
implementation of <see cref="INomadTypeInfoResolver"/>. Applications targeting
NativeAOT or similar ahead-of-time compilation models provide the resolver to
ensure full functionality. The <see cref="ReflectionNomadTypeInfoResolver"/> remains the default for convenient dynamic usage when reflection is permitted. A source generator included in the library emits a `GeneratedNomadTypeInfoResolver` used when no custom resolver is provided. Types may declare `[NomadResolver("My.Custom.Resolver")]` to override the generated resolver.

