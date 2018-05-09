using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DRM.ViewModels;
using DRM_Data.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DRM.Controllers
{
    public class SettingsController : Controller
    {
        private readonly ISettingsManager _settingsManager;

        public SettingsController(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }


        /// <summary>
        /// Returns all known settings
        /// </summary>
        /// <returns></returns>
        [HttpGet("Settings/All")]
        public async Task<IActionResult> GetSettings()
        {
            var result = await _settingsManager.GetSettings();

            SettingsViewModel settings = new SettingsViewModel()
            {
                BackupDirectory = result["BackupDirectory"],
                MailReminderInterval = int.Parse(result["MailReminderInterval"]),
                MailSubscribers = JsonConvert.DeserializeObject<string[]>(result["MailSubscribers"]),
                TaskEvaluationInterval = int.Parse(result["TaskEvaluationInterval"])
            };

            return Ok(settings);
        }

        /// <summary>
        /// Returns all known settings
        /// </summary>
        /// <returns></returns>
        [HttpPost("Settings/Save")]
        public async Task<IActionResult> SaveSettings([FromBody] SettingsViewModel settings)
        {
            await _settingsManager.SaveSetting(nameof(settings.BackupDirectory), settings.BackupDirectory);
            await _settingsManager.SaveSetting(nameof(settings.MailReminderInterval), settings.MailReminderInterval.ToString());
            await _settingsManager.SaveSetting(nameof(settings.MailSubscribers), JsonConvert.SerializeObject(settings.MailSubscribers));
            await _settingsManager.SaveSetting(nameof(settings.TaskEvaluationInterval), settings.TaskEvaluationInterval.ToString());


            return Ok();
        }
    }
}