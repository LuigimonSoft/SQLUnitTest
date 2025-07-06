using System.Threading.Tasks;
using FluentAssertions;
using SQLUnitTest.Models;
using SQLUnitTest.Services;
using SQLUnitTest.Services.Handlers;
using SQLUnitTest.Services.Models;
using Xunit;

namespace SQLUnitTest.Tests.Services
{
    public class BDDTestRunnerTests
    {
        private class FakeHandler : ITestCaseHandler
        {
            public bool CanHandleCalled { get; private set; }
            public bool ExecuteCalled { get; private set; }
            public bool CanHandle(BaseTestCase testCase)
            {
                CanHandleCalled = true;
                return testCase is ExecutionTestCase;
            }
            public Task<TestResult> ExecuteAsync(BaseTestCase testCase)
            {
                ExecuteCalled = true;
                return Task.FromResult(new TestResult { Passed = true, Report = "ok" });
            }
        }

        [Fact]
        public async Task GivenExecutionTestCaseWhenRunTestAsyncShouldInvokeHandlerThenReturnResult()
        {
            var handler = new FakeHandler();
            var runner = new BDDTestRunner(new[] { handler });
            var testCase = new ExecutionTestCase { StoredProcedure = "sp" };

            var result = await runner.RunTestAsync(testCase);

            handler.CanHandleCalled.Should().BeTrue();
            handler.ExecuteCalled.Should().BeTrue();
            result.Passed.Should().BeTrue();
            result.Report.Should().Be("ok");
        }
    }
}
