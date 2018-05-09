using DRM_Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DRM_Data.Interfaces
{
    public interface ISettingsManager
    {
        Task<Dictionary<string, string>> GetSettings();
        Task<bool> SaveSetting(string name, string value);
    }
}
