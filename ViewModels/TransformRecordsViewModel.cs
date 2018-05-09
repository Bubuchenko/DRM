using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DRM.ViewModels
{
    public class TransformRecordsViewModel
    {
        public string ApplicationName { get; set; }
        public int ID { get; set; }
        public List<int> RecordIDs { get; set; }
    }
}
