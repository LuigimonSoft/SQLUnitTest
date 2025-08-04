namespace SQLUnitTest.Models.Mocking
{
    /// <summary>
    /// Represents a single query used for seeding or cleaning data.
    /// </summary>
    public class MockQuery
    {
        /// <summary>
        /// Named connection to run the query against.
        /// </summary>
        public string Connection { get; set; } = string.Empty;

        /// <summary>
        /// SQL statement to execute.
        /// </summary>
        public string Query { get; set; } = string.Empty;

        /// <summary>
        /// Type of precondition to execute. Defaults to <see cref="PreConditionType.Query"/>.
        /// </summary>
        public PreConditionType Type { get; set; } = PreConditionType.Query;
    }
}
