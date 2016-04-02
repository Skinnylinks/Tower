using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Tower.ViewModels
{
    public class NJReportView
    {

       
        public string APN { get; set; }
        public string Township { get; set; }
        public string County { get; set; }
        public string CertificateNumber { get; set; }
        public Nullable<System.DateTime> SaleDateWeb { get; set; }
        public string LienHolder { get; set; }
        [DisplayFormat(DataFormatString = "{0:c}")]
        public Nullable<double> Amount { get; set; }
        public string chg_typ1 { get; set; }
        public string YearInSale { get; set; }
        [DisplayFormat(DataFormatString = "{0:c}")]
        public Nullable<double> Subsequent { get; set; }
        public string chg_typ2 { get; set; }
        public string chg_typ3 { get; set; }
        public string chg_typ4 { get; set; }
        public string chg_typ5 { get; set; }
        public string WEB { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> estRdmDate { get; set; }
        public Nullable<int> age { get; set; }
         [UIHint("YesNo")]
        public Nullable<bool> PaidYN { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> IssuedDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:c}")]
        public Nullable<double> UPDSubs10 { get; set; }
        [DisplayFormat(DataFormatString = "{0:c}")]
        public Nullable<double> UPDSubs11 { get; set; }
        [DisplayFormat(DataFormatString = "{0:c}")]
        public Nullable<double> UPDSubs12 { get; set; }
        [DisplayFormat(DataFormatString = "{0:c}")]
        public Nullable<double> UPDSubs0116 { get; set; }
        [DisplayFormat(DataFormatString = "{0:c}")]
        public Nullable<double> UPDSubs0216 { get; set; }
        [DisplayFormat(DataFormatString = "{0:c}")]
        public Nullable<double> UPDSubs0316 { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> LastUpdate { get; set; }
        public IEnumerable<SelectListItem> qtown { get; set; }
        public string town { get; set; }
    }
}