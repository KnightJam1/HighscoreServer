namespace HighscoreServer.DataServices
{
    /// <summary>
    /// Interface for a data service.
    /// Governs save and load functionality.
    /// </summary>
    public interface IDataService
    {
        /// <summary>
        /// Method to save data.
        /// </summary>
        /// <param name="fileName">Name of the data to be saved.</param>
        /// <param name="data">The data to be saved.</param>
        /// <param name="overwrite">Whether to overwrite existing data.</param>
        void Save(string fileName, Game data, bool overwrite = true);
        
        /// <summary>
        /// Method to load data.
        /// </summary>
        /// <param name="name">Name of the data to be loaded.</param>
        /// <returns>Returns data found with matching name. If there is no matching data, return null.</returns>
        Game Load(string name);
        
        /// <summary>
        /// Initial load. Used when loading for the first time at the start of the project.
        /// </summary>
        /// <param name="name">Name of the data to be loaded.</param>
        /// <returns>Returns the data found or new data if not.</returns>
        Game InitialLoad(string name);
        
        /// <summary>
        /// Delete specified data.
        /// </summary>
        /// <param name="name">Name of data to be deleted.</param>
        void Delete(string name);
        // void DeleteAll();
        
        /// <summary>
        /// List all existing data.
        /// </summary>
        /// <returns>Returns a list of all existing data.</returns>
        IEnumerable<string> ListSaves();
    }
}