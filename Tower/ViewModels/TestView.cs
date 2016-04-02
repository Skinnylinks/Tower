using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Tower.ViewModels
{
    public class TestView
    {
        public int ID { get; set; }
        public Nullable<int> morder { get; set; }
        public string action { get; set; }
        public string pchoice { get; set; }
        public string pchoice1 { get; set; }
        public string pchoice2 { get; set; }
        public string PricingGroup { get; set; }
        [DisplayFormat(DataFormatString = "{0:c}")]
        public Nullable<double> Face_Amount { get; set; }
        [DisplayFormat(DataFormatString = "{0:P2}")]
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
        //public string Today { get; set; }
        public int Months { get; set; }
        public string county { get; set; }
        public string nuser { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        //More
        public string Tax_yr { get; set; }
        public string Cert_Status { get; set; }

    }
}