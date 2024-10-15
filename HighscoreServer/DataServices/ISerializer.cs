namespace HighscoreListener.DataServices
{
    /// <summary>
    /// A serializer to serialize or deserialize data so it can be saved to or loaded from a file.
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Serialize data.
        /// </summary>
        /// <param name="obj">The data to be serialized.</param>
        /// <typeparam name="T">Represents the type of data.</typeparam>
        /// <returns>Returns data as a serialized string.</returns>
        string Serialize<T>(T obj);
        
        /// <summary>
        /// Deserialize data.
        /// </summary>
        /// <param name="serializeText">The serialized text to be deserialized.</param>
        /// <typeparam name="T">The type that the data should be deserialized into.</typeparam>
        /// <returns></returns>
        T Deserialize<T>(string serializeText);
    }
}