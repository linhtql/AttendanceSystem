using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attendance.Service
{
    public abstract class DatabaseServiceBase
    {
        private readonly String connectionString;
        public DatabaseServiceBase(IConfiguration configuration)
        {
            this.connectionString = configuration.GetConnectionString("Test");
        }
        protected async Task<SqlConnection> GetSqlConnectionAsync()
        {
            var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            return connection;
        }

        protected SqlParameter[] GetSqlParameters<T>(T entity) 
        {
            var result = new List<SqlParameter>();

            var entityType = typeof(T);
            var properties = entityType.GetProperties();

            foreach (var property in properties)
            {
                var p = new SqlParameter();
                p.ParameterName = property.Name;
                p.Value = property.GetValue(entity);
                result.Add(p);
            }

            return result.ToArray();
        }
    }
}
