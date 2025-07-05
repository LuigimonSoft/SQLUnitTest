using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SQLUnitTest.Models;
using SQLUnitTest.Services.Handlers;
using SQLUnitTest.Services.Models;

namespace SQLUnitTest.Services
{
    /// <summary>
    /// Simple BDD-style test runner.
    /// </summary>
    public class BDDTestRunner : ITestRunner
    {
        private readonly IEnumerable<ITestCaseHandler> _handlers;

        public BDDTestRunner(IEnumerable<ITestCaseHandler> handlers)
        {
            _handlers = handlers;
        }

        public async Task<TestResult> RunTestAsync(TestCase testCase)
        {
            var handler = _handlers.FirstOrDefault(h => h.CanHandle(testCase));
            if (handler == null)
            {
                return new TestResult { Passed = false, Report = $"Test type {testCase.GetType().Name} not implemented." };
            }

            return await handler.ExecuteAsync(testCase);
        }
    }
}
