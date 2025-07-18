namespace Nomad.Net.Serialization
{
    /// <summary>
    /// Represents structural tokens used by the NOMAD format.
    /// </summary>
    public enum NomadToken : byte
    {
        /// <summary>
        /// Indicates the start of an object.
        /// </summary>
        StartObject = (byte)'{',

        /// <summary>
        /// Indicates the end of an object.
        /// </summary>
        EndObject = (byte)'}',

        /// <summary>
        /// Indicates the start of an array.
        /// </summary>
        StartArray = (byte)'[',

        /// <summary>
        /// Indicates the end of an array.
        /// </summary>
        EndArray = (byte)']',

        /// <summary>
        /// Separates elements in an array.
        /// </summary>
        ValueSeparator = (byte)',',

        /// <summary>
        /// Separates a key from its value.
        /// </summary>
        NameSeparator = (byte)':',
    }
}
