namespace SQLUnitTest.Models
{
    /// <summary>
    /// Validates output parameters of a stored procedure.
    /// </summary>
    public class OutputParameterTestCase : TestCase
    {
        public string StoredProcedure { get; set; } = string.Empty;
        public object? Parameters { get; set; }
        public IDictionary<string, object>? Expected { get; set; }
        public string? Connection { get; set; }
    }
}
