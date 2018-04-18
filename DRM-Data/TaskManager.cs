using DRM_Data.Interfaces;
using DRM_Data.Models;
using DRM_Data.ViewModels;
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

        public async Task<Tuple<bool, string>> CreateDefaultTask(CreateDefaultTaskViewModel task)
        {
            try
            {
                Task t = new Task()
                {
                    Application = await _context.Applications.FindAsync(task.ApplicationID),
                    ColumnName = task.ColumnName,
                    Configuration = await _context.Configurations.FindAsync(task.ConfigurationID),
                    Description = task.Description,
                    Name = task.Name,
                    TableName = task.TableName,
                    Type = Enum.Parse<TaskType>(task.Type, true),
                };

                Condition c = new Condition()
                {
                    Selector = task.FilterColumn,
                    Type = ConditionType.LessThan,
                    Value = task.PeriodInMonths.ToString()
                };

                t.Condition = c;

                await _context.Tasks.AddAsync(t);
                await _context.SaveChangesAsync();

                return Tuple.Create(true, "");
            }
            catch(Exception ex)
            {
                return Tuple.Create(false, ex.Message);
            }
        }

        public Task<Task> GetTaskByID(int id)
        {
            throw new NotImplementedException();
        }
    }
}
