using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DRM_Data.Models
{
    public class Record : Entity
    {
        public string ContentJSON { get; set; }
        public virtual Task Task { get; set; }
        public string PrimaryKeyColumn { get; set; }
        public DateTime? ExecutionDate { get; set; }
        public string Error { get; set; }
        public bool IsCompliant
        {
            get
            {
                return (ExecutionDate != null && ContentJSON == string.Empty);
            }
        }

        public Dictionary<string, object> Content
        {
            get
            {
                return JsonConvert.DeserializeObject<Dictionary<string, object>>(ContentJSON);
            }
        }
    }
}
