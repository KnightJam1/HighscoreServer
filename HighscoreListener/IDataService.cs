using System.Collections.Generic;

namespace SaveLoadSystem
{
    public interface IDataService
    {
        void Save(string fileName, Game data, bool overwrite = true);
        Game? Load(string name);
        void Delete(string name);
        void DeleteAll();
        IEnumerable<string> ListSaves();
    }
}