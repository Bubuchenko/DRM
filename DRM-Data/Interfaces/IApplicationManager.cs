using DRM_Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DRM_Data.Interfaces
{
    public interface IApplicationManager
    {
        Task<List<Application>> GetAllApplications();
        Task<Application> GetApplicationByID(int id);
        Task<Tuple<bool, string>> CreateApplication(Application application);
    }
}
