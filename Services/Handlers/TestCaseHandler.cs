using System.Threading.Tasks;
using SQLUnitTest.Models;
using SQLUnitTest.Services.Models;

namespace SQLUnitTest.Services.Handlers
{
    /// <summary>
    /// Base helper for implementing <see cref="ITestCaseHandler"/> for a specific test type.
    /// </summary>
    /// <typeparam name="T">Type of <see cref="BaseTestCase"/> handled.</typeparam>
    public abstract class TestCaseHandler<T> : ITestCaseHandler where T : BaseTestCase
    {
        public bool CanHandle(BaseTestCase testCase) => testCase is T;

        public Task<TestResult> ExecuteAsync(BaseTestCase testCase)
        {
            return ExecuteAsync((T)testCase);
        }

        protected abstract Task<TestResult> ExecuteAsync(T testCase);
    }
}
