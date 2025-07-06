using System.Collections.Generic;
using SQLUnitTest.Models.Mocking;

namespace SQLUnitTest.Models
{
    /// <summary>
    /// Base class for test case containers.
    /// </summary>
    public class TestCase
    {
        /// <summary>
        /// Optional description of the test.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Context for the test scenario.
        /// </summary>
        public string? Context { get; set; }

        /// <summary>
        /// Object used to mock or seed data prior to execution.
        /// </summary>
        public MockBlock? Mock { get; set; }

        /// <summary>
        /// Collection of nested test cases that describe expectations.
        /// Initialized to an empty list so callers don't need null checks.
        /// </summary>
        public IList<BaseTestCase> Should { get; set; } = new List<BaseTestCase>();
    }
}
