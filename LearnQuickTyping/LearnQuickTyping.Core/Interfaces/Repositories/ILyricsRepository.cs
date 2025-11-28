namespace LearnQuickTyping.Core.Interfaces.Repositories
{
    public interface ILyricsRepository
    {
        IEnumerable<string> GetAllLyricsTitles();
        string[] GetLyricsByIndex(int index);
    }
}
