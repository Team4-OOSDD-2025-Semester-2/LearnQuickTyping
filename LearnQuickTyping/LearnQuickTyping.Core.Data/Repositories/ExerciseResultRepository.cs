using LearnQuickTyping.Core.Data.Database;
using LearnQuickTyping.Core.Interfaces.Database;
using LearnQuickTyping.Core.Interfaces.Repositories;
using LearnQuickTyping.Core.Models;

namespace LearnQuickTyping.Core.Data.Repositories;

public class ExerciseResultRepository : IExerciseResultRepository
{
    private readonly ISqliteConnectionFactory _factory;

    public ExerciseResultRepository(ISqliteConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<int> AddAsync(ExerciseResult result)
    {
        using var conn = await _factory.CreateOpenConnectionAsync();
        using var cmd = conn.CreateCommand();

        cmd.CommandText = @"
            INSERT INTO ExerciseResults (Date, Time, WordsPerMinute, Accuracy, TimeTaken, Errors, ExerciseType, DifficultyLevel)
            VALUES ($date, $time, $wpm, $accuracy, $timetaken, $errors, $type, $difficulty);
            SELECT last_insert_rowid();
        ";

        cmd.Parameters.AddWithValue("$date", result.Date);
        cmd.Parameters.AddWithValue("$time", result.Time);
        cmd.Parameters.AddWithValue("$wpm", result.WordsPerMinute);
        cmd.Parameters.AddWithValue("$accuracy", result.Accuracy);
        cmd.Parameters.AddWithValue("$timetaken", result.TimeTaken);
        cmd.Parameters.AddWithValue("$errors", result.Errors);
        cmd.Parameters.AddWithValue("$type", (int)result.ExerciseType);
        cmd.Parameters.AddWithValue("$difficulty", (int)result.DifficultyLevel);

        var id = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(id);
    }

    public async Task<List<ExerciseResult>> GetAllAsync()
    {
        using var conn = await _factory.CreateOpenConnectionAsync();
        using var cmd = conn.CreateCommand();

        cmd.CommandText = "SELECT * FROM ExerciseResults ORDER BY Date DESC, Time DESC";

        return await ReadResultsAsync(cmd);
    }

    public async Task<List<ExerciseResult>> GetByTypeAsync(ExerciseType type)
    {
        using var conn = await _factory.CreateOpenConnectionAsync();
        using var cmd = conn.CreateCommand();

        cmd.CommandText = "SELECT * FROM ExerciseResults WHERE ExerciseType = $type ORDER BY Date DESC, Time DESC";
        cmd.Parameters.AddWithValue("$type", (int)type);

        return await ReadResultsAsync(cmd);
    }

    public async Task<List<ExerciseResult>> GetRecentAsync(int count = 10)
    {
        using var conn = await _factory.CreateOpenConnectionAsync();
        using var cmd = conn.CreateCommand();

        cmd.CommandText = "SELECT * FROM ExerciseResults ORDER BY Date DESC, Time DESC LIMIT $count";
        cmd.Parameters.AddWithValue("$count", count);

        return await ReadResultsAsync(cmd);
    }

    public async Task<ExerciseResult?> GetBestByTypeAsync(ExerciseType type)
    {
        using var conn = await _factory.CreateOpenConnectionAsync();
        using var cmd = conn.CreateCommand();

        cmd.CommandText = @"
            SELECT * FROM ExerciseResults 
            WHERE ExerciseType = $type 
            ORDER BY WordsPerMinute DESC, Accuracy DESC 
            LIMIT 1
        ";
        cmd.Parameters.AddWithValue("$type", (int)type);

        var results = await ReadResultsAsync(cmd);
        return results.FirstOrDefault();
    }

    public async Task DeleteAsync(int id)
    {
        using var conn = await _factory.CreateOpenConnectionAsync();
        using var cmd = conn.CreateCommand();

        cmd.CommandText = "DELETE FROM ExerciseResults WHERE Id = $id";
        cmd.Parameters.AddWithValue("$id", id);

        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteAllAsync()
    {
        using var conn = await _factory.CreateOpenConnectionAsync();
        using var cmd = conn.CreateCommand();

        cmd.CommandText = "DELETE FROM ExerciseResults";

        await cmd.ExecuteNonQueryAsync();
    }

    private async Task<List<ExerciseResult>> ReadResultsAsync(Microsoft.Data.Sqlite.SqliteCommand cmd)
    {
        var results = new List<ExerciseResult>();

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(new ExerciseResult
            {
                Id = reader.GetInt32(0),
                Date = reader.GetString(1),
                Time = reader.GetString(2),
                WordsPerMinute = reader.GetDouble(3),
                Accuracy = reader.GetInt32(4),
                TimeTaken = reader.GetString(5),
                Errors = reader.GetInt32(6),
                ExerciseType = (ExerciseType)reader.GetInt32(7),
                DifficultyLevel = (DifficultyLevel)reader.GetInt32(8)
            });
        }

        return results;
    }
}