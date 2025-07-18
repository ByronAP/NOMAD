namespace Nomad.Net.Serialization
{
    /// <summary>
    /// Represents the primitive value kinds supported by the binary serializer.
    /// </summary>
    public enum NomadValueKind : byte
    {
        /// <summary>
        /// Indicates a null value.
        /// </summary>
        Null = 0,

        /// <summary>
        /// A 32-bit signed integer.
        /// </summary>
        Int32 = 1,

        /// <summary>
        /// A UTF-8 encoded string.
        /// </summary>
        String = 2,

        /// <summary>
        /// A byte array.
        /// </summary>
        Binary = 3,

        /// <summary>
        /// A boolean value.
        /// </summary>
        Boolean = 4,

        /// <summary>
        /// A 64-bit signed integer.
        /// </summary>
        Int64 = 5,

        /// <summary>
        /// A 32-bit floating point value.
        /// </summary>
        Single = 6,

        /// <summary>
        /// A 64-bit floating point value.
        /// </summary>
        Double = 7,
    }
}
