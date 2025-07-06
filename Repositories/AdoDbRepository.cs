using System;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SQLUnitTest.Repositories
{
    /// <summary>
    /// Basic ADO.NET implementation of <see cref="IDbRepository"/>.
    /// </summary>
    public class AdoDbRepository : IDbRepository
    {
        private readonly IDictionary<string, string> _connections;

        public AdoDbRepository(IDictionary<string, string> connections)
        {
            _connections = connections;
        }

        public async Task<DataSet> ExecuteStoredProcedureAsync(string storedProcedure, object? parameters, string connectionName)
        {
            var ds = new DataSet();
            using var conn = new SqlConnection(_connections[connectionName]);
            using var cmd = new SqlCommand(storedProcedure, conn) { CommandType = CommandType.StoredProcedure };
            if (parameters is IDictionary<string, object> dict)
            {
                foreach (var kvp in dict)
                {
                    cmd.Parameters.AddWithValue($"@{kvp.Key}", kvp.Value ?? DBNull.Value);
                }
            }
            else if (parameters != null)
            {
                foreach (var prop in parameters.GetType().GetProperties())
                {
                    cmd.Parameters.AddWithValue($"@{prop.Name}", prop.GetValue(parameters) ?? DBNull.Value);
                }
            }
            using var da = new SqlDataAdapter(cmd);
            await conn.OpenAsync();
            da.Fill(ds);
            return ds;
        }

        public async Task<DataTable> QueryAsync(string query, string connectionName)
        {
            var dt = new DataTable();
            using var conn = new SqlConnection(_connections[connectionName]);
            using var cmd = new SqlCommand(query, conn);
            using var da = new SqlDataAdapter(cmd);
            await conn.OpenAsync();
            da.Fill(dt);
            return dt;
        }
    }
}
