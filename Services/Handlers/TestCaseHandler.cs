using System.Threading.Tasks;
using SQLUnitTest.Models;
using SQLUnitTest.Services.Models;

namespace SQLUnitTest.Services.Handlers
{
    /// <summary>
    /// Base helper for implementing <see cref="ITestCaseHandler"/> for a specific test type.
    /// </summary>
    /// <typeparam name="T">Type of <see cref="TestCase"/> handled.</typeparam>
    public abstract class TestCaseHandler<T> : ITestCaseHandler where T : TestCase
    {
        public bool CanHandle(TestCase testCase) => testCase is T;

        public Task<TestResult> ExecuteAsync(TestCase testCase)
        {
            return ExecuteAsync((T)testCase);
        }

        protected abstract Task<TestResult> ExecuteAsync(T testCase);
    }
}
