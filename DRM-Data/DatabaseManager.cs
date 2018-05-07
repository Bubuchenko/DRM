using DRM_Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DRM_Data.Models;
using DRM_Data.ViewModels;
using System.Text;
using DRM_Data.DTO;
using System.Linq;
using DRM_Data.Extensions;
using Newtonsoft.Json;
using System.IO;

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
                _conn = new SqlConnection(await GetSQLConnectionString(ConfigurationID));
                await _conn.OpenAsync();

                string sqlQuery = "SELECT TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'";

                using (SqlCommand command = new SqlCommand(sqlQuery, _conn))
                {
                    List<string> tableNames = new List<string>();
                    DataTable dataTable = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(dataTable);
                    foreach (DataRow row in dataTable.Rows)
                    {
                        tableNames.Add($"{row[0]}.{row[1]}");
                    }

                    tableNames.Sort();
                    _conn.Close();
                    return await System.Threading.Tasks.Task.FromResult(Tuple.Create(true, tableNames));
                }
            }
            catch (Exception ex)
            {
                _conn.Close();
                return Tuple.Create(false, new List<string> { ex.Message });
            }
        }

        public async Task<DataTable> GetTableData(GetTableDataParams parameters)
        {
            SqlConnection _conn = new SqlConnection(await GetSQLConnectionString(parameters.ConfigurationID));

            await _conn.OpenAsync();

            StringBuilder sqlQuery = new StringBuilder();

            sqlQuery.Append("SELECT");

            if (parameters.Limit > 0)
                sqlQuery.Append($" TOP { parameters.Limit }");

            sqlQuery.Append($" * FROM { parameters.TableName }");

            if (!string.IsNullOrEmpty(parameters.ActionColumn))
                sqlQuery.Append($" WHERE { parameters.FilterColumn } < DATEADD(month, -{ parameters.Months }, GETDATE())");


            using (SqlCommand command = new SqlCommand(sqlQuery.ToString(), _conn))
            {
                DataTable dataTable = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(dataTable);

                return await System.Threading.Tasks.Task.FromResult(dataTable);
            }
        }

        public async Task<List<string>> GetValidDateTimeFields(GetDateTimeColumnParams parameters)
        {
            SqlConnection _conn = new SqlConnection(await GetSQLConnectionString(parameters.ConfigurationID));

            await _conn.OpenAsync();

            string sqlQuery = $"SELECT TOP 0 * FROM { parameters.TableName }";

            using (SqlCommand command = new SqlCommand(sqlQuery, _conn))
            {
                DataTable dataTable = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(dataTable);

                List<string> dateTimeColumns = new List<string>();

                foreach (DataColumn column in dataTable.Columns)
                    if (column.DataType == typeof(DateTime))
                        dateTimeColumns.Add(column.ColumnName);

                return await System.Threading.Tasks.Task.FromResult(dateTimeColumns);
            }
        }

        public async System.Threading.Tasks.Task EvaluateApplication(int applicationID)
        {
            var application = await _context.Applications.Include(f => f.Tasks).FirstOrDefaultAsync(f => f.ID == applicationID);
            var tasks = _context.Tasks.Where(f => f.Application.ID == applicationID).Include(x => x.Condition).Include(x => x.Configuration);

            ApplicationEvaluationResult result = new ApplicationEvaluationResult
            {
                ApplicationName = application.Name,
                NonCompliantRecordSets = new List<ResultSet>()
            };

            foreach (Task task in tasks)
            {
                SqlConnection _conn = new SqlConnection(await GetSQLConnectionString(task.Configuration.ID));
                await _conn.OpenAsync();
                StringBuilder sqlQuery = new StringBuilder();
                sqlQuery.Append($"SELECT * FROM { task.TableName }");

                switch (task.Type)
                {
                    case TaskType.REMOVE:
                        sqlQuery.Append($" WHERE { task.Condition.Selector } {GetOperator(task.Condition.Type)} DATEADD(month, -{ task.Condition.Value }, GETDATE())");
                        break;
                    case TaskType.NULL:
                        sqlQuery.Append($" WHERE { task.Condition.Selector } {GetOperator(task.Condition.Type)} DATEADD(month, -{ task.Condition.Value }, GETDATE()) AND { task.ColumnName } <> 'NULL'");
                        break;
                    case TaskType.SHA256: //Needs more strict like
                        sqlQuery.Append($" WHERE { task.Condition.Selector } {GetOperator(task.Condition.Type)} DATEADD(month, -{ task.Condition.Value }, GETDATE()) AND { task.ColumnName } LIKE '%[^0-9]%' AND LEN({ task.ColumnName }) <> 64");
                        break;
                    case TaskType.MD5: // Idem
                        sqlQuery.Append($" WHERE { task.Condition.Selector } {GetOperator(task.Condition.Type)} DATEADD(month, -{ task.Condition.Value }, GETDATE()) AND { task.ColumnName } LIKE '%[^0-9]%' AND LEN({ task.ColumnName }) <> 32");
                        break;
                }

                using (SqlCommand command = new SqlCommand(sqlQuery.ToString(), _conn))
                {
                    DataTable dataTable = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(dataTable);

                    var records = dataTable.ToRows();

                    foreach (var record in records)
                    {
                        var content = JsonConvert.SerializeObject(record);
                        var existingRecord = await _context.Records.FirstOrDefaultAsync(f => f.ContentJSON == content && task.ID == task.ID) != null;

                        if (!existingRecord)
                        {
                            _context.Records.Add(new Record()
                            {
                                Task = task,
                                ContentJSON = JsonConvert.SerializeObject(record)
                            });
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task EvaluateApplications()
        {
            var applications = await _context.Applications.Select(f => f.ID).ToListAsync();

            foreach (int applicationID in applications)
            {
                await EvaluateApplication(applicationID);
            }
        }

        public async Task<ApplicationEvaluationResult> GetNonCompliantRecords(int ApplicationID)
        {
            var application = await _context.Applications.FindAsync(ApplicationID);
            var records = await _context.Records.Include(f => f.Task).ThenInclude(x => x.Configuration).Where(f => f.Task.Application.ID == ApplicationID && !f.IsCompliant).ToListAsync();
            var configurations = new List<Configuration>();

            ApplicationEvaluationResult result = new ApplicationEvaluationResult()
            {
                ApplicationName = application.Name,
                NonCompliantRecordSets = new List<ResultSet>()
            };

            foreach (var task in records.Select(f => f.Task).Distinct())
            {
                configurations.Add(task.Configuration);
                var recordSet = new ResultSet()
                {
                    Task = task,
                    Records = new List<(int, Dictionary<string, object>)>()
                };

                foreach (Record record in records.Where(f => f.Task.ID == task.ID))
                {
                    recordSet.Records.Add((record.ID, JsonConvert.DeserializeObject<Dictionary<string, object>>(record.ContentJSON)));
                }

                result.NonCompliantRecordSets.Add(recordSet);
            }

            result.Configurations = configurations.Distinct().ToList();
            return result;
        }

        public async Task<List<ApplicationEvaluationResult>> GetAllNonCompliantRecords()
        {
            var applications = await _context.Applications.Select(f => f.ID).ToListAsync();
            var results = new List<ApplicationEvaluationResult>();

            foreach (int applicationID in applications)
            {
                var result = await GetNonCompliantRecords(applicationID);

                //Only return applications with non compliant records
                if (result.NonCompliantRecordSets.Count > 0)
                    results.Add(await GetNonCompliantRecords(applicationID));
            }
            return results;
        }

        public async Task<(bool, string)> TransformRecord(int RecordID)
        {
            try
            {
                var record = await _context.Records.Include(f => f.Task).ThenInclude(f => f.Configuration).FirstOrDefaultAsync(f => f.ID == RecordID);

                SqlConnection _conn = new SqlConnection(await GetSQLConnectionString(record.Task.Configuration.ID));
                await _conn.OpenAsync();
                StringBuilder sqlQuery = new StringBuilder();

                switch (record.Task.Type)
                {
                    case TaskType.REMOVE:
                        sqlQuery.Append($"DELETE FROM { record.Task.TableName } WHERE");

                        //All column values must match!
                        for (int i = 0; i < record.Content.Count; i++)
                        {
                            sqlQuery.Append($" { record.Content.ElementAt(i).Key } = '{ record.Content.ElementAt(i).Value }'");

                            //Don't append 'AND' if it's the final value in the collection
                            if (i < record.Content.Count - 1)
                                sqlQuery.Append($" AND ");
                        }

                        break;
                    case TaskType.NULL:
                        sqlQuery.Append($"UPDATE { record.Task.TableName } SET { record.Task.ColumnName } = 'null' WHERE");

                        //All column values must match!
                        for (int i = 0; i < record.Content.Count; i++)
                        {
                            sqlQuery.Append($" { record.Content.ElementAt(i).Key } = '{ record.Content.ElementAt(i).Value }'");

                            //Don't append 'AND' if it's the final value in the collection
                            if (i < record.Content.Count - 1)
                                sqlQuery.Append($" AND ");
                        }
                        break;
                    case TaskType.SHA256: //Needs more strict like
                        sqlQuery.Append($"UPDATE { record.Task.TableName } SET { record.Task.ColumnName } = '{ record.Content[record.Task.ColumnName].ToString().ToSHA256() }' WHERE");

                        //All column values must match! 
                        for (int i = 0; i < record.Content.Count; i++)
                        {
                            sqlQuery.Append($" { record.Content.ElementAt(i).Key } = '{ record.Content.ElementAt(i).Value }'");

                            //Don't append 'AND' if it's the final value in the collection
                            if (i < record.Content.Count - 1)
                                sqlQuery.Append($" AND ");
                        }
                        break;
                    case TaskType.MD5: // Idem
                        sqlQuery.Append($"UPDATE { record.Task.TableName } SET { record.Task.ColumnName } = '{ record.Content[record.Task.ColumnName].ToString().ToMD5() }' WHERE");

                        //All column values must match!
                        for (int i = 0; i < record.Content.Count; i++)
                        {
                            sqlQuery.Append($" { record.Content.ElementAt(i).Key } = '{ record.Content.ElementAt(i).Value }'");

                            //Don't append 'AND' if it's the final value in the collection
                            if (i < record.Content.Count - 1)
                                sqlQuery.Append($" AND ");
                        }
                        break;
                }
            }
            catch (Exception ex)
            {

            }


            return (true, "test");
        }

        public async Task<(bool, string)> Backup(int configurationID)
        {
            try
            {
                string BackupDirectory = _context.Settings.FirstOrDefault(f => f.Name == "BackupDirectory").Value;
                var configuration = await _context.Configurations.FindAsync(configurationID);

                var backupFileName = String.Format("{0}{1}-{2}.bak", BackupDirectory, configuration.Database, DateTime.Now.ToString("yyyy-MM-dd hh mm ss"));

                if (!Directory.Exists(BackupDirectory))
                    Directory.CreateDirectory(BackupDirectory);

                using (SqlConnection _conn = new SqlConnection(await GetSQLConnectionString(configuration.ID)))
                {
                    string query = String.Format("BACKUP DATABASE {0} TO DISK='{1}'", configuration.Database, backupFileName);

                    using (var command = new SqlCommand(query, _conn))
                    {
                        await _conn.OpenAsync();
                        command.ExecuteNonQuery();
                    }
                }

                return (true, backupFileName);
            }
            catch(Exception ex)
            {
                return (false, ex.Message);
            }
        }


        private string GetOperator(ConditionType type)
        {
            switch (type)
            {
                case ConditionType.Equals:
                    return "=";
                case ConditionType.GreaterThan:
                    return ">";
                case ConditionType.LessThan:
                    return "<";
                case ConditionType.NotEquals:
                    return "<>";
                default:
                    return "";
            }
        }

        private async Task<string> GetSQLConnectionString(int configurationID)
        {
            Configuration configuration = await _context.Configurations.FirstOrDefaultAsync(f => f.ID == configurationID);

            return
                $"server={configuration.Server};" +
                $"Trusted_Connection=yes;" +
                $"database={configuration.Database};" +
                $"connection timeout=5; " +
                $"integrated security=True";
        }
    }
}
