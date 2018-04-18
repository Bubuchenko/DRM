using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DRM_Data.ViewModels
{
    public class CreateDefaultTaskViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int ConfigurationID { get; set; }
        [Required]
        public int ApplicationID { get; set; }
        [Required]
        public string TableName { get; set; }
        [Required]
        public string ColumnName { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public string FilterColumn { get; set; }
        [Required]
        public int PeriodInMonths { get; set; }
    }
}
