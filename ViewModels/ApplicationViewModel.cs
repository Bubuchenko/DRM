using System;
using System.Collections.Generic;
using System.Text;

namespace DRM.ViewModels
{
    class ApplicationViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }

        public List<TaskViewModel> Tasks { get; set; }

    }
}
