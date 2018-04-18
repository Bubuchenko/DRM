using DRM_Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DRM_Data.Interfaces
{
    public interface IConfigurationManager
    {
        Task<List<Configuration>> GetAllConfigurationsAsync();
        Task<Tuple<bool, string>> CreateConfiguration(Configuration configuration);
        Task<Configuration> GetConfigurationByID(int id);
    }
}
