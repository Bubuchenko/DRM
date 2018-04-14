using DRM_Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRM_Data
{
    public class ApplicationManager : IApplicationManager
    {
        private readonly DRMContext _context;


        public ApplicationManager(DRMContext context)
        {
            _context = context;
        }

        public async Task<Tuple<bool, string>> CreateApplication(Application application)
        {
            try
            {
                var app = _context.Applications.FirstOrDefaultAsync(f => f.Name == application.Name).Result;

                if (app != null)
                    return Tuple.Create<bool, string>(false, "An application with this name already exists.");

                var newApplication = new Application
                {
                    Name = application.Name,
                    Description = application.Description,
                    Configuration = new Configuration()
                };

                await _context.Applications.AddAsync(newApplication);
                await _context.SaveChangesAsync();

                return Tuple.Create<bool, string>(true, null);
            }
            catch(Exception ex)
            {
                //This is unsafe.. but it's a private application, so let's leave it in for easier debugging.
                return Tuple.Create<bool, string>(false, ex.Message);
            }
        }

        public async Task<List<Application>> GetAllApplications()
        {
            return await _context.Applications.Include(f => f.Tasks).ToListAsync();
        }

        public async Task<Application> GetApplicationByID(int id)
        {
            return await _context.Applications.FindAsync(id);
        }
    }
}
