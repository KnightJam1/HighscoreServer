using System.Text.Json;
using HighscoreServer.Loggers;

namespace HighscoreServer.DataServices
{
    /// <summary>
    /// This class handles data by reading and writing to files.
    /// Inherits from IDataService
    /// </summary>
    public class FileDataService : IDataService
    {
        //ISerializer serializer;
        readonly string _dataPath;
        readonly string _fileExtension;
        readonly LoggerTerminal _logger = new LoggerTerminal();

        public FileDataService(string dataDirectory, string fileExtension) // use 'public FileDataService(ISerializer serializer, string dataPath)' for the use of other serializers
        {
            _dataPath = dataDirectory;
            _fileExtension = fileExtension;
            //this.serializer = serializer;
        }

        /// <param name="fileName">The name of the file without an extension</param>
        /// <returns>returns the path to fileName</returns>
        string GetPathToFile(string fileName)
        {
            return Path.Combine(_dataPath, string.Concat(fileName, _fileExtension));
        }

        /// <summary>
        /// Save data to a specified file. Serializes data before saving.
        /// </summary>
        /// <param name="fileName">The name of the file where the data will be saved at. File extension not needed.</param>
        /// <param name="data">The data to be saved.</param>
        /// <param name="overwrite">Determines whether an existing file with the same name should be overwritten.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="IOException"></exception>
        public void Save(string fileName, Game data, bool overwrite = true)
        {
            if (fileName.EndsWith(_fileExtension))
            {
                fileName = fileName.Substring(0, fileName.Length - _fileExtension.Length);
            }
            if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                throw new ArgumentException("Filename contains invalid characters.");
            }
            string fileLocation = GetPathToFile(fileName); //Currently using a default data.json file. Could change to Game.name if game class gets a name property.
            if (!overwrite && File.Exists(fileLocation))
            {
                throw new IOException($"The file '{fileLocation}' already exists and cannot be overwritten."); // Was a throw new IOException, changed to a writeline so the program doesn't end
            }

            File.WriteAllText(fileLocation, JsonSerializer.Serialize(data)); //Or use serializer.Serialize(data) if one is provided
        }

        /// <summary>
        /// Load data from a specified file.
        /// </summary>
        /// <param name="fileName">File data should be loaded from. File extension not needed.</param>
        /// <returns>Returns deserialized data found at the specified location.</returns>
        /// <exception cref="ArgumentException">Throws an argument exception if the filename includes forbidden characters.</exception>
        /// <exception cref="FileNotFoundException">Throws a FileNotFound exception if the specified file does not exist.</exception>
        public Game Load(string fileName)
        {
            if (fileName.EndsWith(_fileExtension))
            {
                fileName = fileName.Substring(0, fileName.Length - _fileExtension.Length);
            }
            if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                throw new ArgumentException("Filename contains invalid characters.");
            }
            string fileLocation = GetPathToFile(fileName);

            if (!File.Exists(fileLocation))
            {
                // Console.WriteLine($"File {fileName} could not be loaded as it does not exist.");
                throw new FileNotFoundException($"Could not find the file '{fileName}' as it does not exist.");
                // return null;
            }
            else
            {
                Game loadedData = JsonSerializer.Deserialize<Game>(File.ReadAllText(fileLocation)) ?? new Game();
                Console.WriteLine($"File {fileName}{_fileExtension} was loaded.");

                return loadedData;
            }
        }

        /// <summary>
        /// Initialize by loading data stored in the default file.
        /// If the default file cannot be found, creates new data.
        /// </summary>
        /// <param name="fileName">The name of the default file.</param>
        /// <returns>Data from default file or new data.</returns>
        public Game InitialLoad(string fileName)
        {
            if (fileName.EndsWith(".json"))
            {
                fileName = fileName.Substring(0, fileName.Length - ".json".Length);
            }
            string fileLocation = GetPathToFile(fileName);

            if (!File.Exists(fileLocation))
            {
                _logger.Log($"No default file was found, created new data."); // Potentially replace with an exception.
                return new Game();
            }
            else
            {
                Game loadedData = JsonSerializer.Deserialize<Game>(File.ReadAllText(fileLocation)) ?? new Game();
                _logger.Log($"File {fileName}{_fileExtension} was loaded.");

                return loadedData;
            }
        }

        /// <summary>
        /// Delete a file
        /// </summary>
        /// <param name="fileName">Name of file to be deleted. Do not include .json.</param>
        public void Delete(string fileName)
        {
            string fileLocation = GetPathToFile(fileName);

            if (File.Exists(fileLocation))
            {
                File.Delete(fileLocation);
            }
        }

        // /// <summary>
        // /// Delete every file in the default data directory.
        // /// Careful, this deletes everything
        // /// </summary>
        // public void DeleteAll()
        // {
        //     foreach (string filePath in Directory.GetFiles(_dataPath))
        //     {
        //         File.Delete(filePath);
        //     }
        // }

        /// <summary>
        /// List all saved data files.
        /// </summary>
        /// <returns>Returns a list of all saved data files without their extensions.</returns>
        public IEnumerable<string> ListSaves()
        {
            foreach (string path in Directory.EnumerateFiles(_dataPath))
            {
                if (Path.GetExtension(path) == _fileExtension)
                {
                    yield return Path.GetFileNameWithoutExtension(path);
                }
            }
        }
    }
}