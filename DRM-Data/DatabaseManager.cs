using DRM_Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using DRM_Data.Models;
using DRM_Data.ViewModels;

namespace DRM_Data
{
    public class DatabaseManager : IDatabaseManager
    {
        private readonly DRMContext _context;

        public DatabaseManager(DRMContext context)
        {
            _context = context;
        }

        public async Task<Tuple<bool, List<string>>> GetTableNames(int ConfigurationID)
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
                List<string> tableNames = new List<string>();

                foreach (DataRow row in dt.Rows)
                {
                    tableNames.Add(row[2].ToString());
                }

                _conn.Close();
                return await System.Threading.Tasks.Task.FromResult(Tuple.Create(true, tableNames));
            }
            catch (Exception ex)
            {
                _conn.Close();
                return Tuple.Create(false, new List<string> { ex.Message });
            }
        }

        public async Task<DataTable> GetTableData(GetTableDataParams parameters)
        {
            SqlConnection _conn = null;
            Configuration configuration = await _context.Configurations.FirstOrDefaultAsync(f => f.ID == parameters.ConfigurationID);

            _conn = new SqlConnection(
                $"server={configuration.Server};" +
                $"Trusted_Connection=yes;" +
                $"database={configuration.Database};" +
                $"connection timeout=30; " +
                $"integrated security=True");

            await _conn.OpenAsync();

            StringBuilder sqlQuery = new StringBuilder(); 
            
            sqlQuery.Append("SELECT");
            
            if(parameters.Limit > 0)
                sqlQuery.Append($" TOP { parameters.Limit }");
            
            sqlQuery.Append($" * FROM { parameters.TableName }");

            if (!string.IsN ullOrEmpty(parameters.ActionColumn))
                sqlQuery.Append($" WHERE { parameters.ActionColumn } < DATEADD(month, -{ parameters.Months }, GETDATE())");


            using (SqlCommand command = new SqlCommand(sqlQuery.ToString(), _conn))
            {
                DataTable dataTable = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(dataTable);

                return await System.Threading.Tasks.Task.FromResult(dataTable);
            }
        }

        public Task<DataTable> GetPreviewData()
        {
            throw new NotImplementedException();
        }
    }
}
