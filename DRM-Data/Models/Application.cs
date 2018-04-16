using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRM_Data
{
    public class Application : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual List<Task> Tasks { get; set; }
    }
}
