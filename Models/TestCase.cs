using System.Collections.Generic;
using SQLUnitTest.Models.Mocking;

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
        /// </summary>
        public IList<TestCase>? Should { get; set; }
    }
}
