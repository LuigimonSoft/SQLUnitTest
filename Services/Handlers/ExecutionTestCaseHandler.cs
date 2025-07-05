using System.Threading.Tasks;
using SQLUnitTest.Models;
using SQLUnitTest.Repositories;
using SQLUnitTest.Reporting;
using SQLUnitTest.Services.Models;

namespace SQLUnitTest.Services.Handlers
{
    /// <summary>
    /// Handles execution-only test cases.
    /// </summary>
    public class ExecutionTestCaseHandler : TestCaseHandler<ExecutionTestCase>
    {
        private readonly IDbRepository _repository;
        private readonly IMarkdownReporter _reporter;

        public ExecutionTestCaseHandler(IDbRepository repository, IMarkdownReporter reporter)
        {
            _repository = repository;
            _reporter = reporter;
        }

        protected override async Task<TestResult> ExecuteAsync(ExecutionTestCase testCase)
        {
            var ds = await _repository.ExecuteStoredProcedureAsync(
                testCase.StoredProcedure,
                testCase.Parameters,
                testCase.Connection ?? "Default");
            var report = _reporter.CreateExecutionReport(testCase, ds);
            return new TestResult
            {
                Passed = true,
                Report = report
            };
        }
    }
}
