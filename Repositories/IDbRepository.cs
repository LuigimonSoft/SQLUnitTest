using System.Data;
using System.Threading.Tasks;

namespace SQLUnitTest.Repositories
{
    /// <summary>
    /// Abstraction over database access.
    /// </summary>
    public interface IDbRepository
    {
        Task<DataSet> ExecuteStoredProcedureAsync(string storedProcedure, object? parameters, string connectionName);
        Task<DataTable> QueryAsync(string query, string connectionName);
    }
}
