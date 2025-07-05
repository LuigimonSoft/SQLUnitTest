namespace SQLUnitTest.Models
{
    /// <summary>
    /// Base class for all test cases.
    /// </summary>
    public abstract class TestCase
    {
        /// <summary>
        /// Optional description of the test.
        /// </summary>
        public string? Description { get; set; }
    }
}
