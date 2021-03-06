﻿using DRM_Data.Interfaces;
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
                    da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                    da.Fill(dataTable);

                    var records = dataTable.ToRows();

                    foreach (var record in records)
                    {
                        var content = JsonConvert.SerializeObject(record);
                        var existingRecord = await _context.Records.FirstOrDefaultAsync(f => f.ContentJSON == content && task.ID == task.ID) != null;

                        if (!existingRecord)
                        {
                            Record newRec = new Record()
                            {
                                Task = task,
                                ContentJSON = content
                            };

                            foreach (DataColumn column in dataTable.Columns)
                            {
                                bool isPrimaryKeyColumn = dataTable.PrimaryKey.Contains(column);

                                if (isPrimaryKeyColumn)
                                    newRec.PrimaryKeyColumn = column.ColumnName;
                            }
                            _context.Records.Add(newRec);
                        }
                        await _context.SaveChangesAsync();
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

            var LastRunSetting = await _context.Settings.FirstOrDefaultAsync(f => f.Name == "LastRun");
            LastRunSetting.Value = DateTime.Now.ToString();
            await _context.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task ShouldEvaluate()
        {
            var LastRunSetting = await _context.Settings.FirstOrDefaultAsync(f => f.Name == "LastRun");
            var TaskInterval = await _context.Settings.FirstOrDefaultAsync(f => f.Name == "TaskEvaluationInterval");

            if (LastRunSetting.Value.Length > 0)
            {
                DateTime LastRunDate = DateTime.Parse(LastRunSetting.Value);
                if ((DateTime.Now - LastRunDate).TotalDays >= double.Parse(TaskInterval.Value))
                {
                    await EvaluateApplications();
                }
            } else
            {
                await EvaluateApplications();
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
                    Records = new List<(int, Dictionary<string, object>, string)>()
                };

                foreach (Record record in records.Where(f => f.Task.ID == task.ID))
                {
                    recordSet.Records.Add((record.ID, JsonConvert.DeserializeObject<Dictionary<string, object>>(record.ContentJSON), record.Error));
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
            Record record = new Record();
            try
            {
                record = await _context.Records.Include(f => f.Task).ThenInclude(f => f.Configuration).FirstOrDefaultAsync(f => f.ID == RecordID);

                using (SqlConnection _conn = new SqlConnection(await GetSQLConnectionString(record.Task.Configuration.ID)))
                {
                    await _conn.OpenAsync();
                    StringBuilder sqlQuery = new StringBuilder();

                    switch (record.Task.Type)
                    {
                        case TaskType.REMOVE:
                            sqlQuery.Append($"DELETE FROM { record.Task.TableName } WHERE");
                            break;
                        case TaskType.NULL:
                            sqlQuery.Append($"UPDATE { record.Task.TableName } SET { record.Task.ColumnName } = 'null' WHERE");

                            break;
                        case TaskType.SHA256: //Needs more strict like
                            sqlQuery.Append($"UPDATE { record.Task.TableName } SET { record.Task.ColumnName } = '{ record.Content[record.Task.ColumnName].ToString().ToSHA256() }' WHERE");

                            break;
                        case TaskType.MD5: // Idem
                            sqlQuery.Append($"UPDATE { record.Task.TableName } SET { record.Task.ColumnName } = '{ record.Content[record.Task.ColumnName].ToString().ToMD5() }' WHERE");
                            break;
                    }

                    sqlQuery.Append($" { record.PrimaryKeyColumn } = '{ record.Content[record.PrimaryKeyColumn] }'");

                    using (var command = new SqlCommand(sqlQuery.ToString(), _conn))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                record.ExecutionDate = DateTime.Now;
            }
            catch (Exception ex)
            {
                record.Error = ex.ToString();
                await _context.SaveChangesAsync();
                return (false, ex.Message);
            }

            record.ContentJSON = string.Empty;
            await _context.SaveChangesAsync();

            return (true, string.Empty);
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
                        command.CommandTimeout = 0;
                        await _conn.OpenAsync();
                        await command.ExecuteNonQueryAsync();
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
                $"Connection timeout=60; " +
                $"integrated security=True";
        }
    }
}
