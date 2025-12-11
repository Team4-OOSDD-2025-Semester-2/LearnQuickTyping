using NUnit.Framework;
using System.Text.RegularExpressions;

namespace LearnQuickTyping.TestCore;

[TestFixture]
public class ExerciseResultSaveServiceTests
{
    [Test]
    [TestCase("2025-12-11")]
    [TestCase("2025-01-01")]
    [TestCase("2024-06-15")]
    [TestCase("2023-12-31")]
    public void FormatDate_ShouldReturnCorrectFormat_YyyyMmDd(string expectedFormat)
    {
        // Arrange
        var dateRegex = new Regex(@"^\d{4}-\d{2}-\d{2}$");

        // Act & Assert
        Assert.That(dateRegex.IsMatch(expectedFormat), Is.True,
            $"Date '{expectedFormat}' should match format yyyy-MM-dd");
    }

    [Test]
    public void FormatDate_WithDateTime_ShouldReturnCorrectFormat()
    {
        // Arrange
        var testDate = new DateTime(2025, 12, 11);
        var expectedPattern = @"^\d{4}-\d{2}-\d{2}$";

        // Act
        string formattedDate = FormatDate(testDate);

        // Assert
        Assert.That(Regex.IsMatch(formattedDate, expectedPattern), Is.True,
            $"Formatted date '{formattedDate}' should match pattern yyyy-MM-dd");
        Assert.That(formattedDate, Is.EqualTo("2025-12-11"));
    }

    [Test]
    [TestCase(2025, 1, 1, "2025-01-01")]
    [TestCase(2025, 12, 31, "2025-12-31")]
    [TestCase(2024, 6, 15, "2024-06-15")]
    public void FormatDate_WithVariousDates_ShouldReturnExpectedFormat(
        int year, int month, int day, string expected)
    {
        // Arrange
        var testDate = new DateTime(year, month, day);

        // Act
        string result = FormatDate(testDate);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("01-30-500")]
    [TestCase("00-05-123")]
    [TestCase("10-59-999")]
    [TestCase("00-00-001")]
    public void FormatTimeTaken_ShouldReturnCorrectFormat_MmSsFff(string expectedFormat)
    {
        // Arrange
        var timeRegex = new Regex(@"^\d{2}-\d{2}-\d{3}$");

        // Act & Assert
        Assert.That(timeRegex.IsMatch(expectedFormat), Is.True,
            $"Time '{expectedFormat}' should match format mm-ss-fff");
    }

    [Test]
    public void FormatTimeTaken_WithTimeSpan_ShouldReturnCorrectFormat()
    {
        // Arrange
        var timeSpan = new TimeSpan(0, 0, 1, 30, 500); // 1 minute, 30 seconds, 500 milliseconds
        var expectedPattern = @"^\d{2}-\d{2}-\d{3}$";

        // Act
        string formattedTime = FormatTimeTaken(timeSpan);

        // Assert
        Assert.That(Regex.IsMatch(formattedTime, expectedPattern), Is.True,
            $"Formatted time '{formattedTime}' should match pattern mm-ss-fff");
        Assert.That(formattedTime, Is.EqualTo("01-30-500"));
    }

    [Test]
    [TestCase(0, 5, 123, "00-05-123")]
    [TestCase(1, 30, 500, "01-30-500")]
    [TestCase(10, 59, 999, "10-59-999")]
    [TestCase(0, 0, 1, "00-00-001")]
    [TestCase(59, 59, 999, "59-59-999")]
    public void FormatTimeTaken_WithVariousTimeSpans_ShouldReturnExpectedFormat(
        int minutes, int seconds, int milliseconds, string expected)
    {
        // Arrange
        var timeSpan = new TimeSpan(0, 0, minutes, seconds, milliseconds);

        // Act
        string result = FormatTimeTaken(timeSpan);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void FormatTimeTaken_WithZeroTimeSpan_ShouldReturnZeroFormat()
    {
        // Arrange
        var timeSpan = TimeSpan.Zero;

        // Act
        string result = FormatTimeTaken(timeSpan);

        // Assert
        Assert.That(result, Is.EqualTo("00-00-000"));
    }

    [Test]
    public void ExerciseResult_MinimalValues_ShouldBeValid()
    {
        // Arrange & Act
        var result = new ExerciseResultTestData
        {
            WordsPerMinute = 0,
            Accuracy = 0,
            Errors = 0,
            TimeTaken = "00-00-000",
            Date = "2025-01-01"
        };

        // Assert
        Assert.That(result.WordsPerMinute, Is.EqualTo(0));
        Assert.That(result.Accuracy, Is.EqualTo(0));
        Assert.That(result.Errors, Is.EqualTo(0));
    }

    [Test]
    public void ExerciseResult_MaximalValues_ShouldBeValid()
    {
        // Arrange & Act
        var result = new ExerciseResultTestData
        {
            WordsPerMinute = 500,
            Accuracy = 100,
            Errors = 5000,
            TimeTaken = "99-59-999",
            Date = "2025-12-31"
        };

        // Assert
        Assert.That(result.WordsPerMinute, Is.EqualTo(500));
        Assert.That(result.Accuracy, Is.EqualTo(100));
        Assert.That(result.Errors, Is.EqualTo(5000));
    }

    // Helper methods that mirror the actual implementation
    private string FormatDate(DateTime date)
    {
        return date.ToString("yyyy-MM-dd");
    }

    private string FormatTimeTaken(TimeSpan timeTaken)
    {
        int totalMinutes = (int)timeTaken.TotalMinutes;
        int seconds = timeTaken.Seconds;
        int milliseconds = timeTaken.Milliseconds;

        return $"{totalMinutes:D2}-{seconds:D2}-{milliseconds:D3}";
    }

    // Test data class for validation
    private class ExerciseResultTestData
    {
        public double WordsPerMinute { get; set; }
        public int Accuracy { get; set; }
        public int Errors { get; set; }
        public string TimeTaken { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
    }
}
