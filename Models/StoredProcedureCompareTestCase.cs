namespace SQLUnitTest.Models
{
    /// <summary>
    /// Compares the results of two stored procedures.
    /// </summary>
    public class StoredProcedureCompareTestCase : BaseTestCase
    {
        public string ExpectedStoredProcedure { get; set; } = string.Empty;
        public object? ExpectedParameters { get; set; }
        public string? ExpectedConnection { get; set; }
        public string[]? ExcludeColumns { get; set; }
    }
}
