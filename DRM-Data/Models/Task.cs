using DRM_Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRM_Data
{
    public class Task : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual Configuration Configuration { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public TaskType Type { get; set; }
        public virtual Condition Condition { get; set; }
        public virtual Application Application { get; set; }
        public virtual List<Record> Records { get; set; }

        public string SQLStatement
        {
            get
            {
                return "";
            }
        }
    }

    public enum TaskType
    {
        REMOVE, //Removes whole record
        MD5, //Hashes specific cell
        SHA256, //Hashes specific cell
        NULL, //Nulls specific cell
    }
}
