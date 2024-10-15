using System.Text.Json;

namespace HighscoreListener
{
    public class FileDataService : IDataService
    {
        //ISerializer serializer;
        string dataPath;
        string fileExtension;
        LoggerTerminal logger = new LoggerTerminal();

        public FileDataService(string dataDirectory) // use 'public FileDataService(ISerializer serializer, string dataPath)' for the use of other serializers
        {
            dataPath = dataDirectory;
            fileExtension = ".json";
            //this.serializer = serializer;
        }

        string GetPathToFile(string fileName)
        {
            return Path.Combine(dataPath, string.Concat(fileName, fileExtension));
        }

        public void Save(string fileName, Game data, bool overwrite = true)
        {
            if (fileName.EndsWith(".json"))
            {
                fileName = fileName.Substring(0, fileName.Length - ".json".Length);
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

        public Game? Load(string fileName)
        {
            if (fileName.EndsWith(".json"))
            {
                fileName = fileName.Substring(0, fileName.Length - ".json".Length);
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
                Console.WriteLine($"File {fileName}{fileExtension} was loaded.");

                return loadedData;
            }
        }

        public Game FirstTimeLoad(string fileName)
        {
            if (fileName.EndsWith(".json"))
            {
                fileName = fileName.Substring(0, fileName.Length - ".json".Length);
            }
            string fileLocation = GetPathToFile(fileName);

            if (!File.Exists(fileLocation))
            {
                Console.WriteLine($"No default file was found, created new data."); // Potentially replace with an exception.
                return new Game();
            }
            else
            {
                Game loadedData = JsonSerializer.Deserialize<Game>(File.ReadAllText(fileLocation)) ?? new Game();
                Console.WriteLine($"File {fileName}{fileExtension} was loaded.");

                return loadedData;
            }
        }

        public void Delete(string fileName)
        {
            string fileLocation = GetPathToFile(fileName);

            if (File.Exists(fileLocation))
            {
                File.Delete(fileLocation);
            }
        }

        public void DeleteAll()
        {
            foreach (string filePath in Directory.GetFiles(dataPath))
            {
                File.Delete(filePath);
            }
        }

        public IEnumerable<string> ListSaves()
        {
            foreach (string path in Directory.EnumerateFiles(dataPath))
            {
                if (Path.GetExtension(path) == fileExtension)
                {
                    yield return Path.GetFileNameWithoutExtension(path);
                }
            }
        }
    }
}