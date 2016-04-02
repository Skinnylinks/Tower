using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tower.ViewModels
{
    public class DrillDownView
    {
        [Required(ErrorMessage = "This value is required!")]
        public string ddparam1 { get; set; }
        public string ddparam2 { get; set; }


    }
}