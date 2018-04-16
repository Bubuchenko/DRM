using AutoMapper;
using DRM_Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRM_Data
{
    public class ConfigurationManager : IConfigurationManager
    {
        private readonly DRMContext _context;


        public ConfigurationManager(DRMContext context)
        {
            _context = context;
        }

        public async Task<Tuple<bool, string>> CreateConfiguration(Configuration configuration)
        {
            try
            {
                var newConfiguration = Mapper.Map<Configuration>(configuration);
                await _context.Configurations.AddAsync(newConfiguration);
                await _context.SaveChangesAsync();

                return Tuple.Create<bool, string>(true, null);
            }
            catch (Exception ex)
            {
                //This is unsafe.. but it's a private application, so let's leave it in for easier debugging.
                return Tuple.Create<bool, string>(false, ex.Message);
            }
        }

        public async Task<List<Configuration>> GetAllConfigurationsAsync()
        {
            return await _context.Configurations.ToListAsync();
        }
    }
}
