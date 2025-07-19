# NOMAD Example Files

This directory contains example NOMAD files demonstrating various features of the format.  Each `.nmd` example has a matching `.json` file that encodes the same data using conventional JSON syntax.

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

### geolocation.nmd
Latitude and longitude readings with timestamps.

## Edge Cases

### empty_values.nmd
Handling of empty arrays, null values, and missing optional fields.

### escaped_strings.nmd
All JSON escape sequences and special characters.

### binary_data.nmd
Binary data with length prefixes.

## Additional Examples

### extra1/readings.nmd
Duplicate of the simple sensor reading example for extended testing.

### extra2/profile.nmd
Duplicate of the complex user profile example for extended testing.

### extra3/strings.nmd
Duplicate of the escaped strings edge case for extended testing.
### scaling/readings_5.nmd
Five sensor readings; still slightly larger than JSON.

### scaling/readings_10.nmd
Ten sensor readings, shows gradual increase in efficiency.

### scaling/readings_25.nmd
Twenty-five sensor readings for mid-sized comparison.

### scaling/readings_50.nmd
Fifty sensor readings; demonstrates clear size savings.

### scaling/readings_75.nmd
Seventy-five sensor readings with noticeable reduction.

### scaling/readings_100.nmd
One hundred sensor readings to highlight scalability.


## Viewing Files

To view the human-readable content of .nmd files, use:
```bash
hexdump -C filename.nmd
```

Or with the NOMAD CLI tool (when available):
```bash
nomad-cli dump filename.nmd
```
