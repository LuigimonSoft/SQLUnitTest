using System.Threading.Tasks;
using SQLUnitTest.Models;
using SQLUnitTest.Services.Models;

namespace SQLUnitTest.Services.Handlers
{
    /// <summary>
    /// Executes a <see cref="TestCase"/>.
    /// </summary>
    public interface ITestCaseHandler
    {
        /// <summary>
        /// Determines if this handler can process the given test case.
        /// </summary>
        bool CanHandle(TestCase testCase);

        /// <summary>
        /// Executes the test case and returns the result.
        /// </summary>
        Task<TestResult> ExecuteAsync(TestCase testCase);
    }
}
