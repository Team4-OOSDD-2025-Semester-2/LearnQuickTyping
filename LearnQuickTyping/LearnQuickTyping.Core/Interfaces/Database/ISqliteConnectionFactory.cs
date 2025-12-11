using Microsoft.Data.Sqlite;

namespace LearnQuickTyping.Core.Interfaces.Database;

public interface ISqliteConnectionFactory
{
    Task<SqliteConnection> CreateOpenConnectionAsync();
}