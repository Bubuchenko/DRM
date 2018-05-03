using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DRM.ViewModels
{
    public class ApplicationEvaluationResultViewModel
    {
        public string ApplicationName { get; set; }
        public int TotalRecords { get; set; }
        public List<ResultSetViewModel> NonCompliantRecordSets { get; set; }

    }

    public class ResultSetViewModel
    {
        public TaskViewModel Task { get; set; }
        public List<Dictionary<string, object>> Records { get; set; }
    }
}
