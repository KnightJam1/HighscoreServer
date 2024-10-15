namespace HighscoreListener.DataServices
{
    public interface IDataService
    {
        void Save(string fileName, Game data, bool overwrite = true);
        Game? Load(string name);
        Game FirstTimeLoad(string name);
        void Delete(string name);
        void DeleteAll();
        IEnumerable<string> ListSaves();
    }
}