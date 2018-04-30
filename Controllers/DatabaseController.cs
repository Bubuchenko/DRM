﻿using System;
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
        /// Evaluates the tasks of an application and returns all the non compliant records. 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> ApplicationEvaluationResult(int id)
        {
            var result = await _context.EvaluateApplication(id);

            return Ok(result);
        }
    }
}