using System;
using System.Collections.Generic;
using System.Text;

namespace DRM_Data.Models
{
    public class Record : Entity
    {
        public string ContentJSON { get; set; }
        public virtual Task Task { get; set; }
        public DateTime? ExecutionDate { get; set; }
        public bool IsCompliant
        {
            get
            {
                return (ExecutionDate != null && ContentJSON == string.Empty);
            }
        }
    }
}
