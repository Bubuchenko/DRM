using System;
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
    public class ConfigurationController : Controller
    {
        private IConfigurationManager _context;

        public ConfigurationController(IConfigurationManager context)
        {
            _context = context;
        }

        public async Task<IActionResult> All()
        {
            var configurations = await _context.GetAllConfigurationsAsync();

            List<ConfigurationViewModel> configurationVMs = Mapper.Map<List<ConfigurationViewModel>>(configurations);

            return Ok(configurationVMs);
        }
    }
}