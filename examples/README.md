# NOMAD Example Files

This directory contains example NOMAD files demonstrating various features of the format.

## File Types

- `.nmd` - NOMAD binary files
- `.hex` - Hexdump of the NOMAD file with annotations
- `.json` - Equivalent JSON representation for comparison

## Simple Examples

### sensor_reading.nmd
Basic sensor data with primitive types (F32, I64, S).

## Complex Examples

### user_profile.nmd
Nested structures with objects, arrays, and optional fields.

## Real-World Examples

### iot_telemetry.nmd
Time-series data from IoT sensors showing NOMAD's efficiency for repeated structures.

### config_file.nmd
Application configuration demonstrating type safety and string escaping.

### log_entries.nmd
Server logs showing how NOMAD handles varied record structures.

## Edge Cases

### empty_values.nmd
Handling of empty arrays, null values, and missing optional fields.

### escaped_strings.nmd
All JSON escape sequences and special characters.

### binary_data.nmd
Binary data with length prefixes.

## Viewing Files

To view the human-readable content of .nmd files, use:
```bash
hexdump -C filename.nmd
```

Or with the NOMAD CLI tool (when available):
```bash
nomad-cli dump filename.nmd
