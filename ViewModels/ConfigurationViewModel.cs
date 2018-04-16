using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DRM.ViewModels
{
    public class ConfigurationViewModel
    {
        public int ID { get; set; }
        public string Server { get; set; }
        public string Database { get; set; }
        public string Logon { get; set; }

    }
}
