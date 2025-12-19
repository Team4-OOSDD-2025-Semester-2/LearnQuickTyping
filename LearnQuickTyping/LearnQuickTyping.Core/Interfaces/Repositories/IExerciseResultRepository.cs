using LearnQuickTyping.Core.Models;

namespace LearnQuickTyping.Core.Interfaces.Repositories;

public interface IExerciseResultRepository
{
    Task<int> AddAsync(ExerciseResult result);
    Task<List<ExerciseResult>> GetAllAsync();
    Task<List<ExerciseResult>> GetByTypeAsync(ExerciseType type);
    Task<List<ExerciseResult>> GetRecentAsync(int count = 10);
    Task<ExerciseResult?> GetBestByTypeAsync(ExerciseType type);
    Task<int> GetCountByDifficultyAsync(ExerciseType type, DifficultyLevel difficulty); 
    Task DeleteAsync(int id);
    Task DeleteAllAsync();
}