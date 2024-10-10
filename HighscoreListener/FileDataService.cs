using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace SaveLoadSystem
{
    public class FileDataService : IDataService
    {
        //ISerializer serializer;
        string dataPath;
        string fileExtension;

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
            string fileLocation = GetPathToFile(fileName); //Currently using a default data.json file. Could change to Game.name if game class gets a name property.
            if (!overwrite && File.Exists(fileLocation))
            {
                throw new IOException($"The file '{fileLocation}' already exists and cannot be overwritten.");
            }

            File.WriteAllText(fileLocation, JsonSerializer.Serialize(data)); //Or use serializer.Serialize(data) if one is provided
        }

        public Game? Load(string fileName)
        {
            string fileLocation = GetPathToFile(fileName);

            if (!File.Exists(fileLocation))
            {
                Console.WriteLine($"File {fileName} could not be loaded as it does not exist.");
                //throw new ArgumentException($"Could not find the file '{fileName}'");
                return null;
            }
            else
            {
                var loadedData = JsonSerializer.Deserialize<Game>(File.ReadAllText(fileLocation)) ?? new Game();

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