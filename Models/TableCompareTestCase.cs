namespace SQLUnitTest.Models
{
    /// <summary>
    /// Compares two tables in the same or different databases.
    /// </summary>
    public class TableCompareTestCase : TestCase
    {
        public string ActualQuery { get; set; } = string.Empty;
        public string ExpectedQuery { get; set; } = string.Empty;
        public string? ActualConnection { get; set; }
        public string? ExpectedConnection { get; set; }
        public string[]? ExcludeColumns { get; set; }
    }
}
