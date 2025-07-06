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
                    AddParameter(cmd, kvp.Key, kvp.Value);
                }
            }
            else if (parameters != null)
            {
                foreach (var prop in parameters.GetType().GetProperties())
                {
                    AddParameter(cmd, prop.Name, prop.GetValue(parameters));
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

        private static void AddParameter(SqlCommand cmd, string name, object? value)
        {
            var param = new SqlParameter($"@{name}", value ?? DBNull.Value)
            {
                SqlDbType = GetSqlDbType(value)
            };
            cmd.Parameters.Add(param);
        }

        private static SqlDbType GetSqlDbType(object? value)
        {
            if (value == null)
            {
                return SqlDbType.Variant;
            }

            var type = Nullable.GetUnderlyingType(value.GetType()) ?? value.GetType();

            return Type.GetTypeCode(type) switch
            {
                TypeCode.Boolean => SqlDbType.Bit,
                TypeCode.Byte => SqlDbType.TinyInt,
                TypeCode.Int16 => SqlDbType.SmallInt,
                TypeCode.Int32 => SqlDbType.Int,
                TypeCode.Int64 => SqlDbType.BigInt,
                TypeCode.Single => SqlDbType.Real,
                TypeCode.Double => SqlDbType.Float,
                TypeCode.Decimal => SqlDbType.Decimal,
                TypeCode.DateTime => SqlDbType.DateTime,
                TypeCode.String => SqlDbType.NVarChar,
                _ => type == typeof(Guid) ? SqlDbType.UniqueIdentifier :
                     type == typeof(byte[]) ? SqlDbType.VarBinary : SqlDbType.Variant
            };
        }
    }
}
