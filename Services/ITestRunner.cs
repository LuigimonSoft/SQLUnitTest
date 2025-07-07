using System.Threading.Tasks;
using SQLUnitTest.Models;
using SQLUnitTest.Services.Models;

namespace SQLUnitTest.Services
{
    /// <summary>
    /// Executes <see cref="TestCase"/> instances.
    /// </summary>
    public interface ITestRunner
    {
        Task<TestResult> RunTestAsync(TestCase testCase);
        Task<TestResult> RunTestAsync(string testCaseJson);
    }
}
