using DRM_Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using DRM_Data.Models;

namespace DRM_Data
{
    public class DatabaseManager : IDatabaseManager
    {
        private readonly DRMContext _context;

        public DatabaseManager(DRMContext context)
        {
            _context = context;
        }

        public async Task<Tuple<bool, object>> GetTables(int ConfigurationID)
        {
            SqlConnection _conn = null;

            try
            {
                Configuration configuration = await _context.Configurations.FirstOrDefaultAsync(f => f.ID == ConfigurationID);

                _conn = new SqlConnection(
                    $"server={configuration.Server};" +
                    $"Trusted_Connection=yes;" +
                    $"database={configuration.Database};" +
                    $"connection timeout=30; " +
                    $"integrated security=True");

                await _conn.OpenAsync();

                DataTable dt = _conn.GetSchema("Tables");
                List<Table> tables = new List<Table>();

                foreach (DataRow row in dt.Rows)
                {
                    Table t = new Table
                    {
                        Name = row[2].ToString(),
                        Columns = await GetAllColumns(_conn, row[2].ToString())
                    };

                    tables.Add(t);
                }

                _conn.Close();
                return await System.Threading.Tasks.Task.FromResult(Tuple.Create(true, (object)tables));
            }
            catch (Exception ex)
            {
                _conn.Close();
                return Tuple.Create(false, (object)new List<string> { ex.Message });
            }
        }

        private async Task<List<string>> GetAllColumns(SqlConnection _conn, string tableName)
        {
            using (SqlCommand command = new SqlCommand($"SELECT TOP 0 * FROM {tableName} WHERE 1 = 2;", _conn))
            {
                var reader = await command.ExecuteReaderAsync();
                await reader.ReadAsync();
                var tableSchema = reader.GetSchemaTable();

                List<string> columns = new List<string>();

                foreach (DataRow row in tableSchema.Rows)
                {
                    columns.Add(row["ColumnName"].ToString());
                }

                reader.Close();
                return await System.Threading.Tasks.Task.FromResult(columns);
            }
        }
    }
}
