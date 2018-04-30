using DRM_Data.Interfaces;
using DRM_Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace DRM_Data
{
    public class QueueManager : IQueueManager
    {
        private readonly DRMContext _context;

        public QueueManager(DRMContext context)
        {
            _context = context;
        }

       



        private async Task<string> GetSQLConnectionString(int configurationID)
        {
            Configuration configuration = await _context.Configurations.FirstOrDefaultAsync(f => f.ID == configurationID);

            return
                $"server={configuration.Server};" +
                $"Trusted_Connection=yes;" +
                $"database={configuration.Database};" +
                $"connection timeout=5; " +
                $"integrated security=True";
        }
    }
}
