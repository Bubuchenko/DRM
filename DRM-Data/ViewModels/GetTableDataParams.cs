using DRM_Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace DRM_Data.ViewModels {
    public class GetTableDataParams
    {
        public int ConfigurationID { get; set; }
        public string TableName { get; set; }
        public string ActionColumn { get; set; }
        public string Action { get; set; }
        public string FilterColumn { get; set; }
        public int Months { get; set; }
        public int Limit { get; set; }
    }
}