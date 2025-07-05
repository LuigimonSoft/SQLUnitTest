using System.Data;
using System.Threading.Tasks;
using SQLUnitTest.Models;
using SQLUnitTest.Repositories;
using SQLUnitTest.Reporting;
using SQLUnitTest.Services.Models;

namespace SQLUnitTest.Services
{
    /// <summary>
    /// Simple BDD-style test runner.
    /// </summary>
    public class BDDTestRunner : ITestRunner
    {
        private readonly IDbRepository _repository;
        private readonly IMarkdownReporter _reporter;

        public BDDTestRunner(IDbRepository repository, IMarkdownReporter reporter)
        {
            _repository = repository;
            _reporter = reporter;
        }

        public async Task<TestResult> RunTestAsync(TestCase testCase)
        {
            switch (testCase)
            {
                case ExecutionTestCase exec:
                    return await RunExecutionTestAsync(exec);
                default:
                    return new TestResult { Passed = false, Report = $"Test type {testCase.GetType().Name} not implemented." };
            }
        }

        private async Task<TestResult> RunExecutionTestAsync(ExecutionTestCase exec)
        {
            var ds = await _repository.ExecuteStoredProcedureAsync(exec.StoredProcedure, exec.Parameters, exec.Connection ?? "Default");
            var report = _reporter.CreateExecutionReport(exec, ds);
            return new TestResult { Passed = true, Report = report };
        }
    }
}
