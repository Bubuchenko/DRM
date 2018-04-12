using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DRM.ViewModels;
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

            foreach(var application in applications)
            {
                var applicationVM = Mapper.Map<ApplicationViewModel>(application);

                applicationVMs.Add(applicationVM);
            }

            return Ok(applicationVMs);
        }

        public async Task<IActionResult> Get(int id)
        {
            var application = await _applicationManager.GetApplicationByID(id);

            var applicationVM = Mapper.Map<ApplicationViewModel>(application);

            return Ok(applicationVM);
        }
    }
}