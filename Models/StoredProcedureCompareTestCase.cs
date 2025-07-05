namespace SQLUnitTest.Models
{
    /// <summary>
    /// Compares the results of two stored procedures.
    /// </summary>
    public class StoredProcedureCompareTestCase : TestCase
    {
        public string StoredProcedure { get; set; } = string.Empty;
        public object? Parameters { get; set; }
        public string ExpectedStoredProcedure { get; set; } = string.Empty;
        public object? ExpectedParameters { get; set; }
        public string? Connection { get; set; }
        public string? ExpectedConnection { get; set; }
        public string[]? ExcludeColumns { get; set; }
    }
}
