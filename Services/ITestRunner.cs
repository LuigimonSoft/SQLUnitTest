using System.Threading.Tasks;
using SQLUnitTest.Models;
using SQLUnitTest.Services.Models;

namespace SQLUnitTest.Services
{
    /// <summary>
    /// Executes <see cref="BaseTestCase"/> instances.
    /// </summary>
    public interface ITestRunner
    {
        Task<TestResult> RunTestAsync(BaseTestCase testCase);
    }
}
