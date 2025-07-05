namespace SQLUnitTest.Services.Models
{
    /// <summary>
    /// Result of a test execution.
    /// </summary>
    public class TestResult
    {
        public bool Passed { get; set; }
        public string Report { get; set; } = string.Empty;
    }
}
