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
