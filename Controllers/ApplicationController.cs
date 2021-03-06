﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DRM.ViewModels;
using DRM_Data;
using DRM_Data.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DRM.Controllers
{
    public class ApplicationController : Controller
    {
        private readonly IApplicationManager _applicationManager;

        public ApplicationController(IApplicationManager applicationManager)
        {
            _applicationManager = applicationManager;
        }

        public async Task<IActionResult> All()
        {
            var applications = await _applicationManager.GetAllApplications();

            List<ApplicationViewModel> applicationVMs = new List<ApplicationViewModel>();

            foreach (var application in applications)
            {
                var applicationVM = Mapper.Map<ApplicationViewModel>(application);
                applicationVM.IsCompliant = await _applicationManager.IsCompliant(application.ID);

                applicationVMs.Add(applicationVM);
            }
            return Ok(applicationVMs);
        }

        public async Task<IActionResult> Create([FromBody] CreateApplicationViewModel application)
        {
            if (ModelState.IsValid)
            {
                if (String.IsNullOrEmpty(application.Name) || application.Name.Length < 3)
                    return BadRequest("Invalid application name.");

                var IsCreated = await _applicationManager.CreateApplication(new Application { Name = application.Name, Description = application.Description });

                if (!IsCreated.Item1)
                    return BadRequest(IsCreated.Item2);

                return Ok();
            }

            return BadRequest(ModelState.Values.Select(f => f.Errors.Select(x => x.ErrorMessage)));
        }

        public async Task<IActionResult> Update([FromBody] UpdateApplicationViewModel application)
        {
            if (ModelState.IsValid)
            {
                if (String.IsNullOrEmpty(application.Name) || application.Name.Length < 3)
                    return BadRequest("Invalid application name.");

                var IsUpdated = await _applicationManager.UpdateApplication(new Application { ID = application.ID, Name = application.Name, Description = application.Description });

                if (!IsUpdated.Item1)
                    return BadRequest(IsUpdated.Item2);

                return Ok();
            }

            return BadRequest(ModelState);
        }

        public async Task<IActionResult> Get(int id)
        {
            var application = await _applicationManager.GetApplicationByID(id);

            var applicationVM = Mapper.Map<ApplicationViewModel>(application);

            return Ok(applicationVM);
        }
    }
}