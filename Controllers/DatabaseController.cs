using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DRM.ViewModels;
using DRM_Data;
using DRM_Data.Interfaces;
using DRM_Data.Models;
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
                var result = await _context.GetTables(id);

                if((bool)result.Item1 == true)
                    return Ok((List<Table>)result.Item2);
                else
                    return BadRequest(ModelState);
            }

            return BadRequest(ModelState);
        }
    }
}