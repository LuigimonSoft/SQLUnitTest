using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
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

        private async Task<TestResult> RunBaseTestAsync(BaseTestCase testCase)
        {
            var handler = _handlers.FirstOrDefault(h => h.CanHandle(testCase));
            if (handler == null)
            {
                return new TestResult { Passed = false, Report = $"Test type {testCase.GetType().Name} not implemented." };
            }

            return await handler.ExecuteAsync(testCase);
        }

        public async Task<TestResult> RunTestAsync(TestCase testCase)
        {
            var sb = new StringBuilder();
            var passed = true;
            foreach (var should in testCase.Should)
            {
                var result = await RunBaseTestAsync(should);
                if (!result.Passed)
                {
                    passed = false;
                }
                sb.AppendLine(result.Report);
            }

            return new TestResult
            {
                Passed = passed,
                Report = sb.ToString().TrimEnd()
            };
        }

        public async Task<TestResult> RunTestAsync(string testCaseJson)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            options.Converters.Add(new BaseTestCaseJsonConverter());

            var testCase = JsonSerializer.Deserialize<TestCase>(testCaseJson, options);
            if (testCase == null)
            {
                throw new JsonException("Unable to deserialize test case");
            }

            return await RunTestAsync(testCase);
        }
    }
}
