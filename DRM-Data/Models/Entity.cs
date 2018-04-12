using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRM_Data
{
    public class Entity
    {
        public Entity()
        {
            CreationDate = DateTime.Now;
        }

        public int ID { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
