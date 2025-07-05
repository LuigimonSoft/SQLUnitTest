using System.Data;
using FluentAssertions;
using SQLUnitTest.Models;
using SQLUnitTest.Reporting;
using Xunit;

namespace SQLUnitTest.Tests.Reporting
{
    public class MarkdownReporterTests
    {
        [Fact]
        public void CreateExecutionReport_IncludesProcedureNameAndResultCount()
        {
            var reporter = new MarkdownReporter();
            var ds = new DataSet();
            ds.Tables.Add(new DataTable());

            var testCase = new ExecutionTestCase { StoredProcedure = "sp_Test" };
            var report = reporter.CreateExecutionReport(testCase, ds);

            report.Should().Contain("sp_Test");
            report.Should().Contain("1 result set");
        }
    }
}
