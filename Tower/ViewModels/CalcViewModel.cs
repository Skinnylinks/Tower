using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
namespace Tower.ViewModels
{
    public class CalcViewModel
    {
        public int ID { get; set; }
        public int DDID { get; set; }
        public int? morder { get; set; }
        public string action { get; set; }
        public string PricingGroup { get; set; }
        public string pchoice { get; set; }
        public string pchoice2 { get; set; }
        [DisplayFormat(DataFormatString = "{0:c}")]
        public Nullable<double> Face_Value { get; set; }
        [DisplayFormat(DataFormatString = "{0:P2}")]
        public Nullable<double> Month_6 { get; set; }
        [DisplayFormat(DataFormatString = "{0:P2}")]
        public Nullable<double> Month_12 { get; set; }
        [DisplayFormat(DataFormatString = "{0:P2}")]
        public Nullable<double> Month_23 { get; set; }
        [DisplayFormat(DataFormatString = "{0:P2}")]
        public Nullable<double> Month_30 { get; set; }
        [DisplayFormat(DataFormatString = "{0:P2}")]
        public Nullable<double> Month_36 { get; set; }
        [DisplayFormat(DataFormatString = "{0:P2}")]
        public Nullable<double> Today { get; set; }
        //for titles
        public string pricegrp { get; set; }
        public string pricegrp1 { get; set; }
        public string head1 { get; set; }
        public string head2 { get; set; }
        public string jvalue { get; set; }
        public string paction { get; set; }
        public string pcounty { get; set; }
        public string p1where { get; set; }
        public string p2where { get; set; }
        public string col1 { get; set; }
        public string col2 { get; set; }
        //public string pCert { get; set; }
        //public string pAccount { get; set; }
        //public string pAccount1 { get; set; }
        public string qparam1 { get; set; }
        public string qparam2 { get; set; }
        public string ddparam1 { get; set; }
        public string ddparam2 { get; set; }
        public string pissdt { get; set; }
        public string county { get; set; }
        public IEnumerable<SelectListItem> qaction { get; set; }
        public IEnumerable<SelectListItem> qcounty { get; set; }
        public IEnumerable<SelectListItem> Cert { get; set; }
        public IEnumerable<SelectListItem> Account { get; set; }
        public IEnumerable<SelectListItem> pchoice3 { get; set; }
        //sorting
        public string sprice { get; set; }
        public string spchoice { get; set; }
        public string spchoice2 { get; set; }
        public string sFace_Value { get; set; }
        public string sMonth_6 { get; set; }
        public string sMonth_12 { get; set; }
        public string sMonth_23 { get; set; }
        public string sMonth_30 { get; set; }
        public string sMonth_36 { get; set; }
        public string sToday { get; set; }

    }
}