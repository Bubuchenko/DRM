using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DRM_Data.Interfaces
{
    public interface IDatabaseManager
    {
        Task<Tuple<bool, object>> GetTables(int ConfigurationID);
    }
}
