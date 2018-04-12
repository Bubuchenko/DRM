using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DRM.ViewModels
{
    public class TaskViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string Type { get; set; }

    }
}
