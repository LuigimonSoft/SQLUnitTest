using System.Data;
using SQLUnitTest.Models;

namespace SQLUnitTest.Reporting
{
    /// <summary>
    /// Generates markdown output for test results.
    /// </summary>
    public interface IMarkdownReporter
    {
        string CreateExecutionReport(ExecutionTestCase test, DataSet result);
    }
}
