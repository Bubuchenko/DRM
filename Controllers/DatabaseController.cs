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
        [HttpGet("Database/TransformRecord")]
        public async Task<IActionResult> TransformRecord(int id)
        {
            var result = await _context.TransformRecord(id);

            return Ok(result);
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