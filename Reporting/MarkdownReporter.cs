using System.Data;
using System.Text;
using SQLUnitTest.Models;

namespace SQLUnitTest.Reporting
{
    /// <summary>
    /// Minimal markdown report generator.
    /// </summary>
    public class MarkdownReporter : IMarkdownReporter
    {
        public string CreateExecutionReport(ExecutionTestCase test, DataSet result)
        {
            var sb = new StringBuilder();
            var title = string.IsNullOrWhiteSpace(test.TestName)
                ? test.StoredProcedure
                : test.TestName;
            sb.AppendLine($"# Feature: {title}");
            sb.AppendLine();
            sb.AppendLine("Execution succeeded with " + result.Tables.Count + " result set(s).");
            return sb.ToString();
        }
    }
}
