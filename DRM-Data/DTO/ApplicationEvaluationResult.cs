using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DRM_Data.DTO
{
    public class ApplicationEvaluationResult
    {
        public Application Application {get; set;}

        public List<(Task, DataTable)> TaskResultSets { get; set; }

    }
}
