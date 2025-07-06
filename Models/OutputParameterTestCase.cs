using System.Collections.Generic;

namespace SQLUnitTest.Models
{
    /// <summary>
    /// Validates output parameters of a stored procedure.
    /// </summary>
    public class OutputParameterTestCase : BaseTestCase
    {
        public IDictionary<string, object>? Expected { get; set; }
    }
}
