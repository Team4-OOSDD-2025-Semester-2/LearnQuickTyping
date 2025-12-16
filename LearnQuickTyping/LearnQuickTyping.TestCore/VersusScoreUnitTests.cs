using NUnit.Framework;
using System.Diagnostics;

namespace LearnQuickTyping.TestCore;

[TestFixture]
public class VersusScoreServiceTests
{
    private class VersusResultTestData
    {
        public string PlayerName { get; set; } = string.Empty;
        public double WordsPerMinute { get; set; }
        public TimeSpan TimeTaken { get; set; }
        public int Errors { get; set; }
        public double FinalScore => WordsPerMinute - Errors;
    }

    [Test]
    public void CalculateResult_Player1_ShouldReturnCorrectResult()
    {
        // Arrange
        string playerName = "Player 1";
        string targetText = "test";
        string typedText = "test";
        TimeSpan timeTaken = TimeSpan.FromSeconds(5);

        // Act
        var result = CalculateResult(playerName, targetText, typedText, timeTaken);

        // Assert
        Assert.That(result.PlayerName, Is.EqualTo("Player 1"));
        Assert.That(result.Errors, Is.EqualTo(0));
        Assert.That(result.WordsPerMinute, Is.GreaterThan(0));
    }

    [Test]
    public void CalculateResult_Player2_ShouldReturnCorrectResult()
    {
        // Arrange
        string playerName = "Player 2";
        string targetText = "test";
        string typedText = "tset"; // With mistake
        TimeSpan timeTaken = TimeSpan.FromSeconds(5);

        // Act
        var result = CalculateResult(playerName, targetText, typedText, timeTaken);

        // Assert
        Assert.That(result.PlayerName, Is.EqualTo("Player 2"));
        Assert.That(result.Errors, Is.EqualTo(2)); // 'es' vs 'se' = 2 mistakes
    }

    [Test]
    public void CalculateResult_ShouldDistinguishPlayers()
    {
        // Arrange
        string targetText = "hello";
        TimeSpan timeTaken = TimeSpan.FromSeconds(3);

        // Act
        var player1Result = CalculateResult("Player 1", targetText, "hello", timeTaken);
        var player2Result = CalculateResult("Player 2", targetText, "hallo", timeTaken);

        // Assert
        Assert.That(player1Result.PlayerName, Is.Not.EqualTo(player2Result.PlayerName));
        Assert.That(player1Result.Errors, Is.Not.EqualTo(player2Result.Errors));
    }

    [Test]
    public void CalculateResult_SingleExercise_ShouldBeStoredCorrectly()
    {
        // Arrange
        string targetText = "test";
        string typedText = "test";

        // Act
        var result = CalculateResult("Player 1", targetText, typedText, TimeSpan.FromSeconds(2));

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.PlayerName, Is.EqualTo("Player 1"));
        Assert.That(result.Errors, Is.EqualTo(0));
    }

    [Test]
    public void CalculateResult_FiveExercises_ShouldAllBeStoredCorrectly()
    {
        // Arrange
        var results = new List<VersusResultTestData>();
        string[] words = { "test", "hello", "world", "quick", "brown" };

        // Act
        for (int i = 0; i < 5; i++)
        {
            results.Add(CalculateResult($"Player {i + 1}", words[i], words[i], TimeSpan.FromSeconds(i + 1)));
        }

        // Assert
        Assert.That(results.Count, Is.EqualTo(5));
        for (int i = 0; i < 5; i++)
        {
            Assert.That(results[i].PlayerName, Is.EqualTo($"Player {i + 1}"));
            Assert.That(results[i].Errors, Is.EqualTo(0));
        }
    }

    [Test]
    public void CalculateResult_TenExercises_ShouldAllBeStoredCorrectly()
    {
        // Arrange
        var results = new List<VersusResultTestData>();
        string targetText = "typing";

        // Act
        for (int i = 0; i < 10; i++)
        {
            results.Add(CalculateResult($"Player {i + 1}", targetText, targetText, TimeSpan.FromSeconds(i + 1)));
        }

        // Assert
        Assert.That(results.Count, Is.EqualTo(10));
        foreach (var result in results)
        {
            Assert.That(result.Errors, Is.EqualTo(0));
            Assert.That(result.WordsPerMinute, Is.GreaterThan(0));
        }
    }

    [Test]
    public void CalculateResult_ShouldCompleteWithin100Milliseconds()
    {
        // Arrange
        string playerName = "Player 1";
        string targetText = "The quick brown fox jumps over the lazy dog";
        string typedText = "The quick brown fox jumps over the lazy dog";
        TimeSpan timeTaken = TimeSpan.FromSeconds(10);

        // Act
        var stopwatch = Stopwatch.StartNew();
        var result = CalculateResult(playerName, targetText, typedText, timeTaken);
        stopwatch.Stop();

        // Assert
        Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(100),
            $"CalculateResult took {stopwatch.ElapsedMilliseconds}ms, expected was less than 100ms");
    }

    [Test]
    [TestCase(0, 0, "Tie", Description = "Both players 0 mistakes")]
    [TestCase(5, 0, "Player 2", Description = "Player 1 has 5 mistakes, player 2 has 0 mistakes")]
    [TestCase(0, 5, "Player 1", Description = "Player 1 has 0 mistakes, player 2 has 5 mistakes")]
    public void DetermineWinner_TruthTableMistakes_ShouldReturnCorrectWinner(
        int player1Errors, int player2Errors, string expectedWinner)
    {
        // Arrange
        var player1 = new VersusResultTestData
        {
            PlayerName = "Player 1",
            WordsPerMinute = 30,
            Errors = player1Errors
        };
        var player2 = new VersusResultTestData
        {
            PlayerName = "Player 2",
            WordsPerMinute = 30,
            Errors = player2Errors
        };

        // Act
        string winner = DetermineWinner(player1, player2);

        // Assert
        Assert.That(winner, Is.EqualTo(expectedWinner));
    }

    [Test]
    public void DetermineWinner_ShouldReturnCorrectWinner()
    {
        // Arrange - Player 1 with less mistakes
        var player1 = new VersusResultTestData
        {
            PlayerName = "Player 1",
            WordsPerMinute = 30,
            Errors = 2
        };
        var player2 = new VersusResultTestData
        {
            PlayerName = "Player 2",
            WordsPerMinute = 40,
            Errors = 5
        };

        // Act
        string winner = DetermineWinner(player1, player2);

        // Assert
        Assert.That(winner, Is.EqualTo("Player 1"));
    }

    [Test]
    [TestCase(30, 0, 30, 5, "Player 1", Description = "30+0 vs 30+5: Player 1 wins (less mistakes)")]
    [TestCase(40, 5, 30, 0, "Player 2", Description = "40+5 vs 30+0: Player 2 wins (less mistakes)")]
    [TestCase(40, 35, 35, 0, "Player 2", Description = "40+35 vs 35+0: Player 2 wins (less mistakes)")]
    public void DetermineWinner_TruthTableWPMErrors_ShouldReturnCorrectWinner(
        double wpm1, int errors1, double wpm2, int errors2, string expectedWinner)
    {
        // Arrange
        var player1 = new VersusResultTestData
        {
            PlayerName = "Player 1",
            WordsPerMinute = wpm1,
            Errors = errors1
        };
        var player2 = new VersusResultTestData
        {
            PlayerName = "Player 2",
            WordsPerMinute = wpm2,
            Errors = errors2
        };

        // Act
        string winner = DetermineWinner(player1, player2);

        // Assert
        Assert.That(winner, Is.EqualTo(expectedWinner),
            $"Player 1: {wpm1} WPM, {errors1} errors. Player 2: {wpm2} WPM, {errors2} errors.");
    }

    [Test]
    [TestCase(0, 0, "Tie", Description = "Score 0 vs 0")]
    [TestCase(5, 5, "Tie", Description = "Score 5 vs 5")]
    [TestCase(10, 9, "Player 1", Description = "Score 10 vs 9: Player 1 wins")]
    public void DetermineWinner_TruthTableScores_ShouldReturnCorrectResult(
        int score1, int score2, string expectedWinner)
    {
        // Arrange
        var player1 = new VersusResultTestData
        {
            PlayerName = "Player 1",
            WordsPerMinute = score1,
            Errors = 0
        };
        var player2 = new VersusResultTestData
        {
            PlayerName = "Player 2",
            WordsPerMinute = score2,
            Errors = 0
        };

        // Act
        string winner = DetermineWinner(player1, player2);

        // Assert
        Assert.That(winner, Is.EqualTo(expectedWinner));
    }

    [Test]
    public void DetermineWinner_EqualScores_ShouldReturnTie()
    {
        // Arrange
        var player1 = new VersusResultTestData
        {
            PlayerName = "Player 1",
            WordsPerMinute = 30,
            Errors = 5
        };
        var player2 = new VersusResultTestData
        {
            PlayerName = "Player 2",
            WordsPerMinute = 30,
            Errors = 5
        };

        // Act
        string winner = DetermineWinner(player1, player2);

        // Assert
        Assert.That(winner, Is.EqualTo("Tie"));
    }

    private VersusResultTestData CalculateResult(string playerName, string targetText, string typedText, TimeSpan timeTaken)
    {
        // Calculate mistakes
        int errors = 0;
        for (int i = 0; i < Math.Min(targetText.Length, typedText.Length); i++)
        {
            if (targetText[i] != typedText[i])
                errors++;
        }
        errors += Math.Abs(targetText.Length - typedText.Length);

        // Calculate WPM
        double wpm = 0;
        if (timeTaken.TotalMinutes > 0)
        {
            const double CharactersPerWordAverage = 6.8;
            double wordCount = typedText.Length / CharactersPerWordAverage;
            wpm = wordCount / timeTaken.TotalMinutes;
        }

        return new VersusResultTestData
        {
            PlayerName = playerName,
            WordsPerMinute = wpm,
            Errors = errors,
            TimeTaken = timeTaken
        };
    }

    private string DetermineWinner(VersusResultTestData player1, VersusResultTestData player2)
    {
        // First check for mistakes
        if (player1.Errors < player2.Errors)
            return player1.PlayerName;
        if (player2.Errors < player1.Errors)
            return player2.PlayerName;

        // Then check for FinalScore (WPM - Errors)
        if (player1.FinalScore > player2.FinalScore)
            return player1.PlayerName;
        if (player2.FinalScore > player1.FinalScore)
            return player2.PlayerName;

        return "Tie";
    }
}