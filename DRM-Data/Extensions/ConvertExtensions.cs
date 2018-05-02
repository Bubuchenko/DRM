using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DRM_Data.Extensions
{
    public static class ConvertExtensions
    {
        public static List<Dictionary<string, object>> ToRows(this DataTable value)
        {
            var rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in value.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in value.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }

            return rows;
        }
    }
}
