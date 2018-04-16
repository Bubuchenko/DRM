using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DRM.ViewModels
{
    public class ValidateTaskBasicsViewModel
    {
        [Required]
        [StringLength(20)]
        [RegularExpression(@"^.{3,}$", ErrorMessage = "The name must contain atleast 3 characters")]
        public string Name { get; set; }
        [Required]
        [StringLength(500)]
        [RegularExpression(@"^.{15,}$", ErrorMessage = "The description must contain atleast 15 characters")]
        public string Description { get; set; }
    }
}
