﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DRM.ViewModels
{
    public class CreateApplicationViewModel
    {
        [Required]
        [StringLength(20)]
        [RegularExpression(@"^.{3,}$", ErrorMessage = "The name requires a minimum amount of 3 characters")]
        public string Name { get; set; }
        [Required]
        [StringLength(500)]
        [RegularExpression(@"^.{30,}$", ErrorMessage = "The description requires a minimum amount of 30 characters")]
        public string Description { get; set; }
    }
}
