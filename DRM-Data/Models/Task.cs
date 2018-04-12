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
        public Configuration Configuration { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public TaskType Type { get; set; }
        public Condition Condition { get; set; }
        public virtual Application Application { get; set; }

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
        Remove, //Removes whole record
        Hash, //Hashes specific cell
        Null, //Nulls specific cell
    }
}
