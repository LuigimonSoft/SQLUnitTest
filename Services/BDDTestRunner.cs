using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SQLUnitTest.Models;
using SQLUnitTest.Models.Mocking;
using SQLUnitTest.Services.Handlers;
using SQLUnitTest.Repositories;
using SQLUnitTest.Services.Models;

namespace SQLUnitTest.Services
{
    /// <summary>
    /// Simple BDD-style test runner.
    /// </summary>
    public class BDDTestRunner : ITestRunner
    {
        private readonly IEnumerable<ITestCaseHandler> _handlers;
        private readonly IDbRepository _repository;

        public BDDTestRunner(IEnumerable<ITestCaseHandler> handlers, IDbRepository repository)
        {
            _handlers = handlers;
            _repository = repository;
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

        private async Task RunPreConditionAsync(MockQuery pre)
        {
            switch (pre.Type)
            {
                case PreConditionType.Query:
                    await _repository.QueryAsync(pre.Query, pre.Connection);
                    break;
                case PreConditionType.StoredProcedure:
                    await _repository.ExecuteStoredProcedureAsync(pre.Query, null, pre.Connection);
                    break;
                case PreConditionType.SqlFile:
                    var sql = File.ReadAllText(pre.Query);
                    await _repository.QueryAsync(sql, pre.Connection);
                    break;
                case PreConditionType.JsonFile:
                    var json = File.ReadAllText(pre.Query);
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    options.Converters.Add(new BaseTestCaseJsonConverter());
                    var fromFile = JsonSerializer.Deserialize<TestCase>(json, options);
                    if (fromFile?.Mock?.PreConditions != null)
                    {
                        foreach (var nested in fromFile.Mock.PreConditions)
                        {
                            await RunPreConditionAsync(nested);
                        }
                    }
                    break;
            }
        }

        public async Task<TestResult> RunTestAsync(TestCase testCase)
        {
            if (testCase.Mock?.PreConditions != null)
            {
                foreach (var pre in testCase.Mock.PreConditions)
                {
                    await RunPreConditionAsync(pre);
                }
            }

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
