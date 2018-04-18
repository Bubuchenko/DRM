using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using DRM_Data.ViewModels;

namespace DRM_Data.Interfaces
{
    public interface IDatabaseManager
    {
        Task<Tuple<bool, List<string>>> GetTableNames(int ConfigurationID);
        Task<DataTable> GetPreviewData();
        Task<DataTable> GetTableData(GetTableDataParams parameters);
    }
}
