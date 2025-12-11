using LearnQuickTyping.Core.Interfaces.Database;

namespace LearnQuickTyping.Core.Data.Database;

public class SqliteSchemaMigrator
{
    private readonly ISqliteConnectionFactory _factory;

    public SqliteSchemaMigrator(ISqliteConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task MigrateAsync()
    {
        using var conn = await _factory.CreateOpenConnectionAsync();
        using var cmd = conn.CreateCommand();

        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS ExerciseResults (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Date TEXT NOT NULL,                  
                Time TEXT NOT NULL,                  
                WordsPerMinute REAL NOT NULL,        
                Accuracy INTEGER NOT NULL,           
                TimeTakenExercise TEXT NOT NULL,     
                Errors INTEGER NOT NULL,             
                ExerciseType INTEGER NOT NULL,       
                DifficultyLevel INTEGER NOT NULL     
            );
            CREATE INDEX IF NOT EXISTS IX_ExerciseResults_Date ON ExerciseResults(Date);
            CREATE INDEX IF NOT EXISTS IX_ExerciseResults_ExerciseType ON ExerciseResults(ExerciseType);
        ";

        await cmd.ExecuteNonQueryAsync();
    }
}