using DRM_Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRM_Data
{
    public class TaskManager : ITaskManager
    {
        private readonly DRMContext _context;


        public TaskManager(DRMContext context)
        {
            _context = context;
        }

        public Task<Tuple<bool, string>> CreateTask(Task application)
        {
            throw new NotImplementedException();
        }

        public Task<Task> GetTaskByID(int id)
        {
            throw new NotImplementedException();
        }
    }
}
