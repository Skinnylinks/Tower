using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Tower.ViewModels
{
    public class selectDropDown
    {

        public string jvalue { get; set; }
        [Required(ErrorMessage = "This value is required!")]
        public string paction { get; set; }
        public string pcounty { get; set; }
        public string p1where { get; set; }
        public string p2where { get; set; }
        public string col1 { get; set; }
        public string col2 { get; set; }
        //[Required(ErrorMessage = "This value is required!")]
        //public string pCert { get; set; }
        //public string pAccount { get; set; }
        [Required(ErrorMessage = "This value is required!")]
        public string qparam1 { get; set; }
        public string qparam2 { get; set; }
        //[Required(ErrorMessage = "This value is required!")]
        public string ddparam1 { get; set; }
        public string ddparam2 { get; set; }
        [Required(ErrorMessage = "This value is required!")]
        public string county { get; set; }
        //public string qaction { get; set; }
        public string pissdt { get; set; }
        public IEnumerable<SelectListItem> pchoice3 { get; set; }
        public IEnumerable<SelectListItem> qaction { get; set; }
        //[Required]
        public IEnumerable<SelectListItem> qcounty { get; set; }
        //[Required]
        // public IEnumerable<SelectListItem> action { get; set; }
        //public IEnumerable<SelectListItem> county { get; set; }
        public IEnumerable<SelectListItem> Cert { get; set; }
        public IEnumerable<SelectListItem> Account { get; set; }
        public IEnumerable<SelectListItem> param1 { get; set; }
        public IEnumerable<SelectListItem> param2 { get; set; }
        public IEnumerable<SelectListItem> issdt { get; set; }
        public IEnumerable<SelectListItem> qparam1DD { get; set; }
        public IEnumerable<SelectListItem> qparam2DD { get; set; }



    }
}