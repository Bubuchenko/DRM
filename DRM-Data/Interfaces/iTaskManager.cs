﻿using DRM_Data;
using DRM_Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DRM_Data.Interfaces
{
    public interface ITaskManager
    {
        Task<Task> GetTaskByID(int id);
        Task<Tuple<bool, string>> CreateDefaultTask(CreateDefaultTaskViewModel task);
    }
}
