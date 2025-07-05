namespace SQLUnitTest.Models
{
    /// <summary>
    /// Validates a stored procedure returning a single table.
    /// </summary>
    public class TableResultTestCase : TestCase
    {
        public string StoredProcedure { get; set; } = string.Empty;
        public object? Parameters { get; set; }
        public string? Connection { get; set; }
        public string? ExpectedQuery { get; set; }
        public string[]? ExcludeColumns { get; set; }
    }
}
