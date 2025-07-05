namespace SQLUnitTest.Models
{
    /// <summary>
    /// Executes a stored procedure and validates that it runs successfully.
    /// </summary>
    public class ExecutionTestCase : TestCase
    {
        public string StoredProcedure { get; set; } = string.Empty;
        public object? Parameters { get; set; }
        public string? Connection { get; set; }
    }
}
