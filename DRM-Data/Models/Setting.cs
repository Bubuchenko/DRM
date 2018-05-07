using DRM_Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace DRM_Data.Models
{
    public class Setting: Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
    }
}
