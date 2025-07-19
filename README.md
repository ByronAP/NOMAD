# NOMAD - Numeric Object Mapping And Data

[![Specification Version](https://img.shields.io/badge/spec-v1.0-blue.svg)](SPECIFICATION.md)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)](CONTRIBUTING.md)

NOMAD is a data serialization format that combines JSON-like structure with binary-encoded values, achieving 50-90% space savings compared to JSON while maintaining structural familiarity.

## Table of Contents

- [Overview](#overview)
- [Key Features](#key-features)
- [Quick Example](#quick-example)
- [Format Comparison](#format-comparison)
- [Specification](#specification)
- [Use Cases](#use-cases)
- [Implementations](#implementations)
- [Tools](#tools)
- [Getting Started](#getting-started)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

## Overview

NOMAD addresses the fundamental trade-off between human readability and storage efficiency. By replacing repeated string field names with integer identifiers and encoding values in binary format, NOMAD achieves:

- **Massive space savings** while retaining JSON-like structure
- **Fast parsing** through type-directed binary extraction
- **Self-contained files** with no external schema dependencies
- **Optional type safety** when validation is needed

## Key Features

### 🚀 Space Efficient

- Integer field IDs replace repeated string keys
- Binary value encoding eliminates text conversion overhead
- 50-90% smaller than JSON for typical datasets

### 🔍 Familiar & Debuggable

- JSON-like syntax that developers already know
- Standard JSON string escaping
- Visible structure with `{`, `}`, `[`, `]` delimiters

### 📦 Self-Contained

- Schema embedded in file header
- No external definition files required
- Version information included

### ⚡ Performance

- Type information enables direct binary reads
- No string parsing for numeric values
- Integer key comparison faster than strings

### 🧮 AOT Friendly
- A source generator produces a `GeneratedNomadTypeInfoResolver` for AOT builds
- Use `NomadResolver("My.Custom.Resolver")` on a type to supply your own resolver
- Works with NativeAOT by providing an `INomadTypeInfoResolver`

### 🛡️ Type Safe

- Optional type validation
- Rich type system including arrays, maps, and optionals
- String encoding specifications

### 🔄 Evolvable

- Built-in versioning support
- Field deprecation mechanisms
- Forward compatibility guidelines

## Quick Example

### Simple Sensor Data

```nomad
@nomad:1
@refs{1:temperature:F32,2:humidity:F32,3:timestamp:I64,4:location:S}
{1:████,2:████,3:████████,4:"Tokyo"}
```

Where:

- `████` represents 4 bytes of binary float data
- `████████` represents 8 bytes of binary integer data
- `"Tokyo"` is a regular JSON string
- Structure uses familiar `{`, `:`, `,`, `}` syntax

### Human-Readable Representation

```json
{
  "temperature": 23.5,
  "humidity": 45.2,
  "timestamp": 1634567890,
  "location": "Tokyo"
}
```

## Format Comparison

### File Size Comparison (1000 Records)

| Format | Size | Reduction vs JSON | Structure | Self-Contained |
|--------|------|-------------------|-----------|----------------|
| JSON | 116 KB | - | Text | ✓ |
| XML | 191 KB | -65% | Text | ✓ |
| CSV | 42 KB | 64% | Text | ✗ |
| Protocol Buffers | 24 KB | 79% | Binary | ✗ |
| **NOMAD** | **31 KB** | **73%** | **JSON-like** | **✓** |

### Feature Comparison

| Feature | JSON | Protocol Buffers | MessagePack | NOMAD |
|---------|------|------------------|-------------|--------|
| Human Readable Structure | ✓ | ✗ | ✗ | ✓ |
| Binary Efficient Values | ✗ | ✓ | ✓ | ✓ |
| Self-Describing | ✓ | ✗ | Partial | ✓ |
| Type Safety | ✗ | ✓ | ✗ | Optional |
| Schema Evolution | ✗ | ✓ | ✗ | ✓ |

## Specification

The complete NOMAD specification is available in [SPECIFICATION.md](SPECIFICATION.md).

### Type System Overview

**Primitive Types:**

- Integers: `I8`, `I16`, `I32`, `I64`, `U8`, `U16`, `U32`, `U64`
- Floating Point: `F32`, `F64`, `D` (decimal)
- Text/Binary: `S` (string), `X` (binary), `C` (char), `R` (rune)
- Other: `B` (boolean), `N` (null)

**Complex Types:**

- Arrays: `[type]` (e.g., `[I32]` for array of int32)
- Maps: `{key:value}` (e.g., `{S:F64}` for string to float64 map)
- Optional: `?type` (e.g., `?S` for optional string)

### Structural Syntax

NOMAD uses JSON-like structural characters:

| Character | Purpose |
|-----------|---------|
| `{` `}` | Object delimiters |
| `[` `]` | Array delimiters |
| `:` | Key-value separator |
| `,` | Element separator |
| `"` | String delimiter |

Strings use standard JSON escaping (`\"`, `\\`, `\n`, `\r`, `\t`, `\uHHHH`).

## Use Cases

NOMAD excels in scenarios requiring both efficiency and inspectability:

### 📊 Time-Series Data

- IoT sensor readings
- System metrics
- Financial tick data
- Server logs

### 🔧 Configuration Files

- Application settings with type safety
- Multi-environment configurations
- Feature flags with validation

### 🌐 Data Interchange

- API responses with reduced bandwidth
- Message queuing with compact payloads
- Cross-platform data exchange

### 🔌 Embedded Systems

- Constrained memory environments
- Simple parser implementation
- Efficient storage on flash memory

### 📡 Network Protocols

- Binary efficiency over the wire
- Debuggable with packet analyzers
- Self-describing messages

## Implementations

### Official Implementations

| Language | Status | Repository | Package |
|----------|--------|------------|---------|
| Python | Planned | - | - |
| C# | Prototype | [src/Nomad.Net](src/Nomad.Net) | - |
| Go | Planned | - | - |
| Rust | Planned | - | - |
| JavaScript/TypeScript | Planned | - | - |
| C | Planned | - | - |

The C# implementation is documented in [IMPLEMENTATION.md](IMPLEMENTATION.md).

### Community Implementations

See [implementations/](implementations/) for a list of community-maintained implementations.

### Implementation Requirements

All implementations **MUST**:

- Support all primitive types
- Handle JSON-like structural syntax correctly
- Support JSON string escaping
- Use varint encoding for field headers (max 10 bytes)
- Validate version headers
- Provide clear error messages
- Be compatible with ahead-of-time (AOT) compilation via compile-time metadata
  supplied through `INomadTypeInfoResolver` generated by the source generator or custom implementations

## Tools

### Included Tools

- `nomad-report` - Generates size comparison tables for the example data sets. The tool runs against each set to aid regression testing and public comparisons.

### Planned Tools

- `nomad-cli` - Command-line tool for working with NOMAD files
  - Convert between NOMAD and JSON/CSV
  - Validate NOMAD files
  - Pretty-print NOMAD data
  - Generate schema documentation

- `nomad-viewer` - GUI application for viewing NOMAD files
- `nomad-bench` - Benchmarking tool for implementations

## Getting Started

### Basic Concepts

1. **Field IDs**: Instead of `{"name": "John", "age": 30}`, NOMAD uses `{1: "John", 2: 30}`
2. **Reference Section**: Maps IDs to names and types: `@refs{1:name:S,2:age:I32}`
3. **Binary Values**: Numbers stored in native binary format, not text
4. **JSON-like Structure**: Uses familiar `{}`, `[]`, `:`, `,` syntax
5. **Type Safety**: Optional validation ensures data matches expected types

### Example: Converting JSON to NOMAD

**Original JSON:**

```json
{
  "sensor_id": "TEMP-001",
  "location": "Server Room A",
  "readings": [
    {"value": 23.5, "timestamp": 1634567890},
    {"value": 24.1, "timestamp": 1634567950}
  ]
}
```

**Complete NOMAD file:**

```nomad
@nomad:1
@refs{1:sensor_id:S,2:location:S,3:readings:[{4:F32,5:I64}],4:value:F32,5:timestamp:I64}
{1:"TEMP-001",2:"Server Room A",3:[{4:████,5:████████},{4:████,5:████████}]}
```

Where:

- Strings use JSON syntax with quotes
- `████` = 4 bytes for each F32 value (23.5, 24.1)
- `████████` = 8 bytes for each I64 timestamp
- Structure is identical to JSON except integer keys

### Another Example: Configuration with Special Characters

**JSON:**

```json
{
  "app_name": "My App",
  "base_path": "C:\\Program Files\\MyApp",
  "description": "Line 1\nLine 2\tTabbed",
  "port": 8080,
  "debug": true
}
```

**Complete NOMAD file:**

```nomad
@nomad:1
@refs{1:app_name:S,2:base_path:S,3:description:S,4:port:U16,5:debug:B}
{1:"My App",2:"C:\\Program Files\\MyApp",3:"Line 1\nLine 2\tTabbed",4:██,5:█}
```

Where:

- Strings use JSON escaping (note the `\\` and `\n`)
- `██` = 2 bytes for U16 port number (8080)
- `█` = 1 byte for boolean (0x01 = true)

### Binary Data Example

**Binary data (type X) is encoded as raw bytes:**

```nomad
@nomad:1
@refs{1:config:S,2:icon:X,3:signature:X}
{1:"app.ico",2:████...binary data...,3:████████████████████}
```

Where:

- `████...binary data...` = icon bytes (length defined by schema)
- `████████████████████` = 16 bytes of signature
- Binary data can contain any bytes including structural characters

### Reading NOMAD Files (Pseudocode)

```python
def read_value(field_type):
    if field_type in INTEGER_TYPES:
        return read_bytes(size_of(field_type))
    elif field_type in FLOAT_TYPES:
        return read_bytes(size_of(field_type))
    elif field_type == 'B':
        return read_byte() != 0
    elif field_type == 'S':
        return read_json_string()  # Handles escaping
    elif field_type == 'X':
        length = expected_length(field_type)  # Defined by schema or structure
        return read_bytes(length)
    # ... handle other types

# Parse structure like JSON
def parse_object():
    expect('{')
    obj = {}
    while peek() != '}':
        field_id = read_integer()
        expect(':')
        field_type = refs[field_id].type
        value = read_value(field_type)
        obj[refs[field_id].name] = value
        if peek() == ',':
            consume(',')
    expect('}')
    return obj
```

### Complex Example: User Records

**JSON:**

```json
{
  "users": [
    {
      "id": 12345,
      "name": "Alice",
      "email": "alice@example.com",
      "avatar": "<binary data>",
      "preferences": {
        "theme": "dark",
        "notifications": true
      }
    }
  ]
}
```

**NOMAD:**

```nomad
@nomad:1
@refs{
  1:users:[{2:U64,3:S,4:S,5:X,6:{7:S,8:B}}],
  2:id:U64,
  3:name:S,
  4:email:S,
  5:avatar:X,
  6:preferences,
  7:theme:S,
  8:notifications:B
}
{1:[{2:████████,3:"Alice",4:"alice@example.com",5:██...avatar...,6:{7:"dark",8:█}}]}
```

## Contributing

We welcome contributions to NOMAD! See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

### Ways to Contribute

- 🐛 Report bugs and issues
- 💡 Suggest new features
- 📝 Improve documentation
- 🔧 Submit implementations
- 🧪 Add test cases
- 🌍 Translate documentation

### Development Process

1. Check existing issues and discussions
2. Open an issue for major changes
3. Fork and create a feature branch
4. Make changes with clear commits
5. Add tests and documentation
6. Submit a pull request

## License

This project is licensed under the MIT License - see [LICENSE](LICENSE) for details.

## Contact

- **Specification Issues**: [GitHub Issues](https://github.com/ByronAP/NOMAD/issues)
- **General Discussion**: [GitHub Discussions](https://github.com/ByronAP/NOMAD/discussions)
- **Security Issues**: Please report security vulnerabilities privately to [security contact]

## Acknowledgments

NOMAD combines ideas from several excellent formats:

- JSON for familiar syntax and escaping rules
- Protocol Buffers for integer field identifiers
- MessagePack for efficient binary encoding
- CSV for tabular data efficiency

---

<p align="center">
  Made with ❤️ for efficient data serialization
</p>
