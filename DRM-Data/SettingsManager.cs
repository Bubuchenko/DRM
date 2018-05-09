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
    public class SettingManager : ISettingsManager
    {
        private readonly DRMContext _context;

        public SettingManager(DRMContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<string, string>> GetSettings()
        {
            var settingsDictionary = new Dictionary<string, string>();
            var settings = await _context.Settings.ToListAsync();

            foreach (var setting in settings)
                settingsDictionary.Add(setting.Name, setting.Value);

            return settingsDictionary;
        }

        public async Task<bool> SaveSetting(string name, string value)
        {
            try
            {
                var setting = await _context.Settings.FirstOrDefaultAsync(f => f.Name == name);
                setting.Value = value;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
