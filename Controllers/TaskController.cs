using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DRM.ViewModels;
using DRM_Data;
using DRM_Data.Interfaces;
using DRM_Data.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DRM.Controllers
{
    public class TaskController : Controller
    {
        private readonly ITaskManager _taskManager;

        public TaskController(ITaskManager taskManager)
        {
            _taskManager = taskManager;
        }

        public IActionResult ValidateBasics([FromBody] ValidateTaskBasicsViewModel application)
        {
            if (ModelState.IsValid)
            {
                return Ok();
            }

            return BadRequest(ModelState);
        }

        public async Task<IActionResult> CreateTask([FromBody] CreateDefaultTaskViewModel taskViewModel)
        {
            if (ModelState.IsValid)
            {
                var createTaskResult = await _taskManager.CreateDefaultTask(taskViewModel);
                if (createTaskResult.Item1)
                    return Ok();
                else
                    return BadRequest(createTaskResult.Item2);
            }

            return BadRequest(ModelState);
        }
    }
}
