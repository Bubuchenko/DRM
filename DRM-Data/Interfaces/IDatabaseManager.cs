using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using DRM_Data.DTO;
using DRM_Data.ViewModels;

namespace DRM_Data.Interfaces
{
    public interface IDatabaseManager
    {
        Task<Tuple<bool, List<string>>> GetTableNames(int ConfigurationID);
        Task<DataTable> GetTableData(GetTableDataParams parameters);
        Task<List<string>> GetValidDateTimeFields(GetDateTimeColumnParams parameters);
        void EvaluateApplication(int applicationID);
        void EvaluateApplications();
        Task<ApplicationEvaluationResult> GetNonCompliantRecords(int ApplicationID);
        Task<List<ApplicationEvaluationResult>> GetAllNonCompliantRecords();
    }
}
