# NOMAD: Numeric Object Mapping And Data

## Status of This Document

This document specifies the NOMAD (Numeric Object Mapping And Data) format version 1.0. This is an independent specification.

## Abstract

This document defines NOMAD, a data serialization format that combines JSON-like structural elements with binary-encoded values. NOMAD uses integer field identifiers in place of string field names to achieve significant space savings while maintaining structural visibility through familiar JSON syntax.

## Table of Contents

1. [Introduction](#1-introduction)
2. [Conventions and Terminology](#2-conventions-and-terminology)
3. [NOMAD Format Specification](#3-nomad-format-specification)
4. [Type System](#4-type-system)
5. [Encoding Rules](#5-encoding-rules)
6. [Parsing Rules](#6-parsing-rules)
7. [Schema Evolution](#7-schema-evolution)
8. [Conformance Requirements](#8-conformance-requirements)
9. [Security Considerations](#9-security-considerations)
10. [IANA Considerations](#10-iana-considerations)
11. [Examples](#11-examples)
12. [References](#12-references)
13. [Appendix A: Implementation Guidelines](#appendix-a-implementation-guidelines)

---

## 1. Introduction

### 1.1. Purpose

NOMAD addresses the need for a data serialization format that provides:

- Space efficiency through integer field references and binary value encoding
- Structural familiarity using JSON-like syntax
- Self-contained schema information
- Type safety when required
- Simple implementation requirements

### 1.2. Design Goals

The design goals for NOMAD are:

1. Minimal overhead for field identification
2. Binary efficiency for value storage
3. Familiar JSON-like syntax for structure
4. Optional type validation
5. No external schema dependencies

### 1.3. Relation to Other Formats

NOMAD combines concepts from several existing formats:

- JSON: Familiar syntax and string escaping
- Protocol Buffers: Integer field identifiers
- MessagePack: Binary value encoding
- CSV: Tabular data efficiency

## 2. Conventions and Terminology

### 2.1. Requirements Notation

The key words "**MUST**", "**MUST NOT**", "**REQUIRED**", "**SHALL**", "**SHALL NOT**", "**SHOULD**", "**SHOULD NOT**", "**RECOMMENDED**", "**NOT RECOMMENDED**", "**MAY**", and "**OPTIONAL**" in this document are to be interpreted as described in RFC 2119 [RFC2119].

### 2.2. Terminology

- **Field**: A named data element identified by an integer
- **Reference Section**: The section containing field-to-type mappings
- **Data Section**: The section containing encoded values
- **Type Specifier**: A shorthand notation indicating data type
- **Structural Characters**: The characters `{`, `}`, `[`, `]`, `:`, `,`
- **Varint**: Variable-length integer encoding

### 2.3. Notational Conventions

- Hexadecimal values are written with a "0x" prefix
- Binary data in examples is shown as █ characters for non-printable bytes
- `<LF>` represents line feed (0x0A)

## 3. NOMAD Format Specification

### 3.1. Overall Structure

A NOMAD file **MUST** consist of exactly three sections in the following order:

1. Version Header
2. Reference Section  
3. Data Section

The file **MUST** begin with the ASCII characters "@nomad:" followed by a version number.

### 3.2. Version Header

#### 3.2.1. Syntax

The version header **MUST** have the following format:

```abnf
version-header = "@nomad:" major-version LF
major-version = 1*DIGIT
LF = %x0A  ; Line feed
```

#### 3.2.2. Version Rules

- The major version indicates the format version
- Parsers **MUST** reject files with different major versions
- New major versions indicate incompatible changes

#### 3.2.3. Example

```json
@nomad:1
@refs{...}
```

### 3.3. Reference Section

#### 3.3.1. Syntax

The reference section **MUST** begin with "@refs{" and end with "}". The section **MUST** contain zero or more field definitions separated by commas.

```abnf
reference-section = "@refs{" [field-list] "}"
field-list = field-definition *("," field-definition)
field-definition = integer ":" field-name ":" type-specifier
integer = 1*DIGIT
field-name = identifier / quoted-string
identifier = ALPHA *(ALPHA / DIGIT / "_")
quoted-string = DQUOTE *(CHAR / escaped-char) DQUOTE
```

#### 3.3.2. Field Identifiers

Field identifiers **MUST** be positive integers (1-2147483647). The value 0 is **RESERVED** and **MUST NOT** be used. Each identifier **MUST** be unique for a given type. Field identifiers **SHOULD** be assigned sequentially starting from 1, but gaps are permitted.

#### 3.3.3. Field Names

Field names **MUST** be unique within a reference section. Field names containing whitespace or special characters **MUST** be enclosed in double quotes. Standard JSON string escaping rules **MUST** be applied within quoted strings.

### 3.4. Data Section

#### 3.4.1. Structure

The data section uses JSON-like syntax with the following structural characters:

| Character | ASCII | Hex | Purpose |
|-----------|-------|-----|---------|
| `{` | 123 | 0x7B | Object start |
| `}` | 125 | 0x7D | Object end |
| `[` | 91 | 0x5B | Array start |
| `]` | 93 | 0x5D | Array end |
| `:` | 58 | 0x3A | Key-value separator |
| `,` | 44 | 0x2C | Element separator |
| `"` | 34 | 0x22 | String delimiter |

#### 3.4.2. Object Encoding

Objects **MUST** be encoded as:

```json
{ field-id : value [, field-id : value]* }
```

#### 3.4.3. Field ID Encoding in Data Section

Field identifiers in the data section **MUST** be encoded as unquoted decimal integers.

Examples:

- Field 1: `1` (not "1")
- Field 42: `42` (not "42")

## 4. Type System

### 4.1. Type Specifiers

The following type specifiers are defined:

#### 4.1.1. Integer Types

| Specifier | Type | Size | Range |
|-----------|------|------|-------|
| I8 | Signed 8-bit | 1 byte | -128 to 127 |
| I16 | Signed 16-bit | 2 bytes | -32,768 to 32,767 |
| I32 | Signed 32-bit | 4 bytes | -2^31 to 2^31-1 |
| I64 | Signed 64-bit | 8 bytes | -2^63 to 2^63-1 |
| U8 | Unsigned 8-bit | 1 byte | 0 to 255 |
| U16 | Unsigned 16-bit | 2 bytes | 0 to 65,535 |
| U32 | Unsigned 32-bit | 4 bytes | 0 to 2^32-1 |
| U64 | Unsigned 64-bit | 8 bytes | 0 to 2^64-1 |

#### 4.1.2. Floating Point Types

| Specifier | Type | Size | Standard |
|-----------|------|------|----------|
| F32 | 32-bit float | 4 bytes | IEEE 754-2019 binary32 |
| F64 | 64-bit float | 8 bytes | IEEE 754-2019 binary64 |
| D | Decimal | Variable | Implementation-defined |

#### 4.1.3. Other Primitive Types

| Specifier | Type | Encoding |
|-----------|------|----------|
| B | Boolean | 1 byte (0x00=false, 0x01=true) |
| S | String | UTF-8 with JSON escaping |
| X | Binary | Raw bytes |
| C | Character | Single byte |
| R | Rune | 4-byte Unicode code point (UTF-32) |
| O | Object | Runtime-determined encoding |
| N | Null | No bytes (presence only) |

### 4.2. Complex Types

Complex types **MUST** be specified using the following syntax:

- Arrays: `[element-type]`
- Maps: `{key-type:value-type}`
- Optional: `?type`
- Type reference: `#integer`

### 4.3. Type Modifiers

#### 4.3.1. Endianness

Integer and floating-point types **MAY** include an endianness modifier:

- No suffix: Native byte order
- 'b' suffix: Big-endian (e.g., I32b)
- 'l' suffix: Little-endian (e.g., I32l)

When no endianness is specified, implementations **SHOULD** use little-endian for maximum portability.

#### 4.3.2. String Encodings

String types **MAY** include an encoding modifier:

- S: UTF-8 (default)
- SA: ASCII (7-bit)
- S1: ISO-8859-1 (Latin-1)
- S6: UTF-16LE
- S6b: UTF-16BE

## 5. Encoding Rules

### 5.1. Fixed-Size Types

Fixed-size numeric types **MUST** be encoded in their binary representation according to their specified byte order. Boolean values **MUST** be encoded as a single byte with value 0x00 (false) or 0x01 (true). Any non-zero value **SHOULD** be interpreted as true.

### 5.2. Variable-Size Types

#### 5.2.1. String Encoding

Strings **MUST** be enclosed in double quotes and use standard JSON escape sequences:

| Character | Escape Sequence |
|-----------|----------------|
| Quotation mark | `\"` |
| Backslash | `\\` |
| Backspace | `\b` |
| Form feed | `\f` |
| Line feed | `\n` |
| Carriage return | `\r` |
| Tab | `\t` |
| Unicode | `\uHHHH` |

Control characters (U+0000 through U+001F) **MUST** be escaped.

#### 5.2.2. Binary Encoding

Binary data (type X) **MUST** be encoded as raw bytes without any length prefix. The surrounding JSON structure or schema determines how many bytes belong to the value.

A reader derives the byte count from its context:

* For fields with a fixed-size type, the declared size defines the length.
* Within arrays or objects, the closing `]` or `}` delimiter marks the end of the value.
* For top-level values, the schema or enclosing file format specifies the expected length.

### 5.3. Null Values

Fields with type specifier 'N' or nullable fields with null values **MUST** be encoded as the JSON literal `null` without quotes.

Example: `{1:"data",2:null,3:████}`

### 5.4. Array Encoding

Arrays **MUST** be encoded using square brackets with comma-separated elements:

```json
[ element [, element]* ]
```

Empty arrays **MUST** be encoded as: `[]`

### 5.5. Map Encoding

Maps **MUST** be encoded as objects where keys and values alternate:

```json
{ key : value [, key : value]* }
```

Empty maps **MUST** be encoded as: `{}`

### 5.6. Dynamic Values

Fields declared with the `O` type specifier **MAY** contain any value. Primitive
values **MUST** begin with a `NomadValueKind` marker followed by the encoded
bytes for that kind. Arrays and objects use the standard structural tokens and
MAY nest other dynamic values recursively. Parsers **SHOULD** materialize such
values using generic collections (e.g., `List<object?>` for arrays and
`Dictionary<int, object?>` for objects).

## 6. Parsing Rules

### 6.1. Parser Requirements

A conforming NOMAD parser **MUST**:

1. Verify the version header and check compatibility
2. Parse the complete reference section before the data section
3. Build a type lookup table from the reference section
4. Use type information to determine value byte consumption
5. Recognize all structural characters
6. Handle JSON string escape sequences
7. Handle varint decoding for field headers
8. Enforce the 10-byte maximum for varint decoding

### 6.2. Error Handling

#### 6.2.1. Fatal Errors

The following conditions **MUST** be treated as fatal errors:

- Missing or malformed version header
- Incompatible major version
- Missing or malformed reference section
- Invalid JSON structure
- Premature end of file
- Field ID of 0
- Varint longer than 10 bytes

#### 6.2.2. Recoverable Errors

The following conditions **SHOULD** be handled gracefully:

- Unknown field identifiers (skip field)
- Type mismatches (implementation-defined behavior)
- Fields present that are marked deprecated

> **Implementation Note:**
> The reference .NET library included in this repository skips unknown fields
> encountered during deserialization.

### 6.3. Parsing Algorithm

```mermaid
flowchart TB
    Start([Start])
    Start --> CheckVersion{Check @nomad:}
    CheckVersion -->|Invalid| Error([Fatal Error])
    CheckVersion -->|Valid| ParseVersion[Parse Version]
    ParseVersion --> CheckCompat{Compatible?}
    CheckCompat -->|No| Error
    CheckCompat -->|Yes| ParseRefs[Parse @refs Section]
    ParseRefs --> BuildTable[Build Type Table]
    BuildTable --> ParseData[Parse Data Section]
    
    ParseData --> ReadChar[Read Character]
    ReadChar --> IsObject{Is '{' ?}
    IsObject -->|Yes| ParseObject[Parse Object]
    IsObject -->|No| IsArray{Is '[' ?}
    IsArray -->|Yes| ParseArray[Parse Array]
    IsArray -->|No| Done([Done])
    
    ParseObject --> ReadFieldID[Read Field ID]
    ReadFieldID --> ExpectColon[Expect ':']
    ExpectColon --> LookupType[Lookup Field Type]
    LookupType --> ReadValue[Read Typed Value]
    ReadValue --> IsComma{Is ',' ?}
    IsComma -->|Yes| ReadFieldID
    IsComma -->|No| ExpectClose{Is '}' ?}
    ExpectClose -->|Yes| ParseData
    ExpectClose -->|No| Error
```

## 7. Schema Evolution

### 7.1. Forward Compatibility

To maintain forward compatibility:

1. New fields **SHOULD** be added with higher field IDs
2. Existing field IDs **MUST NOT** be reused for different purposes
3. Field types **MUST NOT** be changed (deprecate and add new)
4. Optional types **SHOULD** be used for new fields when possible

### 7.2. Deprecation

Deprecated fields **SHOULD** be marked in the reference section using a comment syntax:

```json
@refs{
  1:active:B,
  2:old_name:S,  // @deprecated Use field 4
  3:age:I32,
  4:full_name:S
}
```

### 7.3. Field ID Assignment Strategy

The following strategies are **RECOMMENDED**:

1. **Reserved Ranges**: Reserve ID ranges for different categories
   - 1-999: Core fields
   - 1000-1999: Extension fields
   - 2000-9999: Application-specific fields

2. **Sequential Assignment**: Assign IDs sequentially as fields are added

## 8. Conformance Requirements

### 8.1. Writer Conformance

A conforming NOMAD writer **MUST**:

1. Generate a valid version header
2. Generate valid reference sections with unique field names
3. Use only defined type specifiers
4. Encode values according to their declared types
5. Use correct JSON structural characters
6. Properly escape strings using JSON rules
7. Encode binary data as raw bytes
8. Produce files with the .nmd extension

### 8.2. Reader Conformance

A conforming NOMAD reader **MUST**:

1. Verify the version header
2. Check version compatibility
3. Parse the complete reference section before data
4. Support all primitive types
5. Handle JSON string escape sequences
6. Implement varint decoding for field headers
7. Implement error recovery for unknown fields
8. Enforce the 10-byte maximum for varint decoding

### 8.3. Type Validation

Implementations that support type validation **MUST**:

1. Verify values match declared types
2. Enforce structure constraints for validated types
3. Report type mismatches appropriately
4. Validate string encodings

## 9. Security Considerations

### 9.1. Resource Exhaustion

Implementations **MUST** guard against resource exhaustion attacks:

- Field ID 0 **MUST** be rejected
- Varint decoding **MUST NOT** exceed 10 bytes
- Binary data lengths **MUST** be validated before allocation

Implementations **SHOULD** implement additional limits:

- Maximum nesting depth (recommend 1000 levels)
- Maximum string length
- Maximum binary data size
- Maximum total memory consumption

### 9.2. Implementation-Specific Limits

Like JSON parsers, NOMAD parsers **MAY** impose implementation-specific limits. Common limits include:

- Maximum nesting depth (typically 100-1000 levels)
- Maximum string length
- Maximum binary data size
- Maximum array/map size
- Maximum number of unique field IDs

These limits **SHOULD** be documented by the implementation and **SHOULD** produce clear error messages when exceeded.

### 9.3. String Validation

Implementations **MUST**:

- Validate UTF-8 sequences in strings
- Reject unescaped control characters
- Validate escape sequences
- Check for invalid Unicode code points in `\uHHHH` escapes

### 9.4. Binary Data Validation

Implementations **MUST**:

- Check for integer overflow in length calculations
- Ensure binary data does not exceed stated length
- Validate length against available input

## 10. IANA Considerations

### 10.1. Media Type Registration

This document registers the following media type:

- Type name: application
- Subtype name: x-nomad
- Required parameters: none
- Optional parameters:
  - charset: Character encoding for reference section (default "UTF-8")
- Encoding considerations: binary
- Security considerations: See Section 9
- Published specification: This document
- File extension: .nmd
- Macintosh file type code: 'NOMD'
- Magic number: "@nomad:" (0x40 0x6E 0x6F 0x6D 0x61 0x64 0x3A)

## 11. Examples

### 11.1. Simple Sensor Data

```json
@nomad:1
@refs{1:temperature:F32,2:humidity:F32,3:timestamp:I64}
{1:████,2:████,3:████████}
```

Where binary values are:

- Field 1: 4 bytes float (23.5)
- Field 2: 4 bytes float (45.0)
- Field 3: 8 bytes integer (1634567890)

### 11.2. Complete Example with All Types

```json
@nomad:1
@refs{
  1:id:U64,
  2:name:S,
  3:active:B,
  4:score:F32,
  5:data:X,
  6:tags:[S],
  7:metadata:{S:S},
  8:optional_note:?S
}
{
  1:████████,
  2:"Alice Smith",
  3:█,
  4:████,
  5:██████████...raw bytes...,
  6:["user","admin","verified"],
  7:{"role":"manager","dept":"engineering"},
  8:null
}
```

Where field 5 (binary data) has:

- `████████...raw bytes...` = actual binary data

### 11.3. Nested Structure

```json
@nomad:1
@refs{
  1:users:[{2:S,3:S,4:?{5:S,6:S}}],
  2:name:S,
  3:email:S,
  4:address,
  5:street:S,
  6:city:S
}
{
  1:[
    {2:"John",3:"john@example.com",4:null},
    {2:"Jane",3:"jane@example.com",4:{5:"123 Main St",6:"Boston"}}
  ]
}
```

### 11.4. String Escaping Examples

```json
@nomad:1
@refs{1:path:S,2:description:S,3:json:S}
{
  1:"C:\\Program Files\\MyApp",
  2:"Line 1\nLine 2\tTabbed",
  3:"{\"key\":\"value\"}"
}
```

### 11.5. Binary Data Example

```json
@nomad:1
@refs{1:image_id:S,2:thumbnail:X,3:checksum:X}
{
  1:"IMG_001",
  2:█████...PNG data...,
  3:████████████████████
}
```

Where:

- Field 2: `█████...PNG data...` = thumbnail bytes
- Field 3: `████████████████████` = 16 bytes of MD5 checksum

### 11.6. File Size Comparison

For 1000 sensor records with fields: temperature, humidity, timestamp, location

| Format | Header | Structure | Field Names | Values | Total | Reduction |
|--------|--------|-----------|-------------|---------|-------|-----------|
| JSON | 0 | 11,000 | 67,000 | 38,000 | 116,000 | - |
| XML | 43 | 19,000 | 134,000 | 38,000 | 191,043 | - |
| CSV | 39 | 4,000 | 0 | 38,000 | 42,039 | 64% |
| NOMAD | 59 | 11,000 | 0 | 20,000 | 31,059 | **73%** |

## 12. References

### 12.1. Normative References

[RFC2119] Bradner, S., "Key words for use in RFCs to Indicate Requirement Levels", BCP 14, RFC 2119, March 1997.

[RFC7159] Bray, T., Ed., "The JavaScript Object Notation (JSON) Data Interchange Format", RFC 7159, March 2014.

[IEEE754] IEEE, "Standard for Floating-Point Arithmetic", IEEE 754-2019.

[UTF-8] Yergeau, F., "UTF-8, a transformation format of ISO 10646", RFC 3629, November 2003.

### 12.2. Informative References

[JSON] ECMA-404, "The JSON Data Interchange Standard", October 2017.

[MessagePack] MessagePack Specification, https://msgpack.org/

[ProtoBuf] Protocol Buffers, https://protobuf.dev/

## Appendix A: Implementation Guidelines

### A.1. Suggested Defaults

While limits are implementation-specific, these defaults are suggested as a starting point:

| Element | Suggested Default |
|---------|------------------|
| Maximum nesting depth | 1000 |
| Maximum string length | 100 MB |
| Maximum binary field size | 100 MB |
| Maximum array/map elements | 10 million |
| Maximum unique field IDs | 65,536 |

### A.2. Performance Considerations

For optimal performance, implementations **SHOULD**:

1. Cache the parsed reference section for reuse
2. Use memory-mapped I/O for large files
3. Implement streaming parsers where possible
4. Pre-allocate buffers based on type information
5. Use lookup tables for field type resolution

### A.3. Debugging Support

Implementations are encouraged to provide:

1. Human-readable dump utilities
2. Validation tools
3. Format conversion utilities (to/from JSON)
4. Pretty-printing with field names

### A.4. JSON Compatibility Note

While NOMAD uses JSON-like syntax, it is **NOT** valid JSON due to:

- Unquoted integer keys
- Binary values embedded directly
- The @nomad and @refs headers

Tools **SHOULD NOT** attempt to parse NOMAD files with standard JSON parsers.

---

**Document Version**: 1.0  
**Date**: July 2025  
**Authors**: Allen Byron Penner (ByronAP)  
**Status**: Proposed Standard
