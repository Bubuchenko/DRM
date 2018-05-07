using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRM_Data
{
    public class Configuration : Entity
    {
        public string Server { get; set; }
        public string Database { get; set; }
        [JsonIgnore]
        public string Logon { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
    }
}
