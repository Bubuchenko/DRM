using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DRM.ViewModels;
using DRM_Data;
using DRM_Data.Interfaces;
using DRM_Data.Models;
using DRM_Data.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DRM.Controllers
{
    public class RunningTask
    {
        public int ID { get; set; }
        public bool IsCancelled { get; set; }
        public int totalItems { get; set; }
        public int completedItems { get; set; }
        public IDictionary<int, string> FailedItems { get; set; }
    }

    public static class DatabaseActionsState
    {
        public static List<RunningTask> RunningTasks = new List<RunningTask>();
    }

    public class DatabaseController : Controller
    {
        private IDatabaseManager _context;


        public DatabaseController(IDatabaseManager context)
        {
            _context = context;
        }

        public async Task<IActionResult> Tables(int id)
        {
            if (ModelState.IsValid)
            {
                var result = await _context.GetTableNames(id);

                if ((bool)result.Item1 == true)
                    return Ok(result.Item2);
                else
                    return BadRequest(ModelState);
            }

            return BadRequest(ModelState);
        }

        public async Task<IActionResult> TableData([FromBody] GetTableDataParams parameters)
        {
            if (ModelState.IsValid)
            {
                DataTable dt = await _context.GetTableData(parameters);

                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                Dictionary<string, object> row;
                foreach (DataRow dr in dt.Rows)
                {
                    row = new Dictionary<string, object>();
                    foreach (DataColumn col in dt.Columns)
                    {
                        row.Add(col.ColumnName, dr[col]);
                    }
                    rows.Add(row);
                }

                return Ok(rows);
            }

            return BadRequest(ModelState);
        }

        public async Task<IActionResult> WhereColumns([FromBody] GetDateTimeColumnParams parameters)
        {
            if (ModelState.IsValid)
            {
                return Ok(await _context.GetValidDateTimeFields(parameters));
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Returns all currently stored non compliant records from all tasks, from a specific application
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Database/NCRecords")]
        public async Task<IActionResult> GetApplicationNCRecords(int id)
        {
            var result = Mapper.Map<ApplicationEvaluationResultViewModel>(await _context.GetNonCompliantRecords(id));

            //Count all the records for quick overview 
            result.TotalRecords = result.NonCompliantRecordSets.Select(f => f.Records.Count).Sum();

            return Ok(result);
        }

        /// <summary>
        /// Returns all currently stored non compliant records from all tasks, from all applications
        /// </summary>
        /// <returns></returns>
        [HttpGet("Database/AllNCRecords")]
        public async Task<IActionResult> GetAllNonCompliantRecords()
        {
            var result = Mapper.Map<List<ApplicationEvaluationResultViewModel>>(await _context.GetAllNonCompliantRecords());

            //Count all the records for quick overview 
            result.ForEach(f => f.TotalRecords = f.NonCompliantRecordSets.Select(i => i.Records.Count).Sum());

            return Ok(result);
        }

        /// <summary>
        /// Returns all currently stored non compliant records from all tasks, from all applications
        /// </summary>
        /// <returns></returns>
        [HttpPost("Database/TransformRecord")]
        public async Task<IActionResult> TransformRecord([FromBody] PostIDViewModel id)
        {
            var result = await _context.TransformRecord(id.ID);

            await System.Threading.Tasks.Task.Delay(1000);

            return Ok(result);
        }

        /// <summary>
        /// Transforms all specified records
        /// </summary>
        /// <returns></returns>
        [HttpPost("Database/TransformRecords")]
        public async Task<IActionResult> TransformRecords([FromBody] TransformRecordsViewModel transformParams)
        {
            RunningTask task = new RunningTask()
            {
                ID = transformParams.ID,
                FailedItems = new Dictionary<int, string>(),
                totalItems = transformParams.RecordIDs.Count
            };

            DatabaseActionsState.RunningTasks.Add(task);

            var queuedTask = DatabaseActionsState.RunningTasks.FirstOrDefault(f => f.ID == task.ID);

            await System.Threading.Tasks.Task.Delay(2000);

            for (var i = 0; i < transformParams.RecordIDs.Count; i++)
            {
                if (task.IsCancelled)
                    break;

                var record = transformParams.RecordIDs[i];
                var result = await _context.TransformRecord(record);

                if (result.Item1 == false)
                    queuedTask.FailedItems.Add(record, result.Item2);

                queuedTask.completedItems++;
            }

            return Ok();
        }

        [HttpGet("Database/RunningTasks")]
        public async Task<IActionResult> RunningTasks()
        {
            await System.Threading.Tasks.Task.Delay(1);
            return Ok(DatabaseActionsState.RunningTasks);
        }

        [HttpGet("Database/GetTaskProgress")]
        public async Task<IActionResult> TaskProgress(int id)
        {
            await System.Threading.Tasks.Task.Delay(0);
            return Ok(DatabaseActionsState.RunningTasks.FirstOrDefault(f => f.ID == id));
        }

        /// <summary>
        /// Evaluates all tasks of all applicationns
        /// </summary>
        /// <returns></returns>
        [HttpGet("Database/EvaluateAll")]
        public async Task<IActionResult> EvaluateApplications()
        {
            await _context.EvaluateApplications();

            return Ok();
        }

        /// <summary>
        /// Backs up the database linked to the configuration of the specified task
        /// </summary>
        /// <returns></returns>
        [HttpPost("Database/Backup")]
        public async Task<IActionResult> Backup([FromBody] PostIDViewModel vm)
        {
            var result = await _context.Backup(vm.ID);

            if (result.Item1)
                return Ok(result.Item1);

            return BadRequest(result.Item2);
        }
    }
}