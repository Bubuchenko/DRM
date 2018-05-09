using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DRM.ViewModels
{
    public class SettingsViewModel
    {
        public string BackupDirectory { get; set; }
        public int TaskEvaluationInterval { get; set; }
        public string[] MailSubscribers { get; set; }
        public int MailReminderInterval { get; set; }

    }
}
