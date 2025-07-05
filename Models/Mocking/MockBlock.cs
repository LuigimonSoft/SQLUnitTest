using System.Collections.Generic;

namespace SQLUnitTest.Models.Mocking
{
    /// <summary>
    /// Block containing pre-condition queries for a test.
    /// </summary>
    public class MockBlock
    {
        /// <summary>
        /// Queries to execute before running the test.
        /// </summary>
        public IList<MockQuery>? PreConditions { get; set; }
    }
}
