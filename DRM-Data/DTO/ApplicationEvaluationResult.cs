using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DRM_Data.DTO
{
    public class ApplicationEvaluationResult
    {
        public string ApplicationName { get; set; }
        public List<ResultSet> NonCompliantRecordSets { get; set; }

    }

    public class ResultSet
    {
        public Task Task { get; set; }
        public List<Dictionary<string, object>> Records {get; set; }
    }
}
