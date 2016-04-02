﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tower.ViewModels
{
    public class NJDownloadView
    {
        public Nullable<double> Sort_No { get; set; }
        public string APN { get; set; }
        public string Township { get; set; }
        public string County { get; set; }
        public string Block { get; set; }
        public string Lot { get; set; }
        public string Qualifier { get; set; }
        public string Additional_Lots { get; set; }
        public Nullable<double> Amount { get; set; }
        public string Type { get; set; }
        public string Notes { get; set; }
        public string HouseNo { get; set; }
        public string DirL { get; set; }
        public string StreetName { get; set; }
        public string StreetSufx { get; set; }
        public string DirR { get; set; }
        public string Unit { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZIP { get; set; }
        public string FullLocation { get; set; }
        public string CityStateZip { get; set; }
        public string Owner { get; set; }
        public string AbsentOwner { get; set; }
        public string Class { get; set; }
        public string LandUse { get; set; }
        public string Zoning { get; set; }
        public string BldgDescr { get; set; }
        public string NoStories { get; set; }
        public string NoUnits { get; set; }
        public string YearBuilt { get; set; }
        public string LotWidth { get; set; }
        public string LotDepth { get; set; }
        public string LotSize { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string SalePrice { get; set; }
        public string SaleDate { get; set; }
        public string PriorSalePrice { get; set; }
        public string PriorSaleDate { get; set; }
        public string AsmntYear { get; set; }
        public string LandValue { get; set; }
        public string ImpValue { get; set; }
        public string TotalValue { get; set; }
        public string TaxYear { get; set; }
        public string Taxes { get; set; }
        public string C1stMtgAmt { get; set; }
        public string C1stMtgRate { get; set; }
        public string C1stMtgTerm { get; set; }
        public string C1stMtgType { get; set; }
        public string C2ndMtgAmt { get; set; }
        public string C2ndMtgRate { get; set; }
        public string C2ndMtgTerm { get; set; }
        public string C2ndMtgType { get; set; }
        public string MailOwner { get; set; }
        public string MailCO { get; set; }
        public string MailAddress { get; set; }
        public string MailCityState { get; set; }
        public string MailZip { get; set; }
        public string OwnerPhone { get; set; }
        public string Owner_Name_Address_ { get; set; }
        public string CertificateNumber { get; set; }
        public Nullable<System.DateTime> SaleDateWeb { get; set; }
        public string LienHolder { get; set; }
        public Nullable<double> SaleAmount { get; set; }
        public string chg_typ1 { get; set; }
        public string YearInSale { get; set; }
        public Nullable<double> Subsequent { get; set; }
        public string chg_typ2 { get; set; }
        public string chg_typ3 { get; set; }
        public string chg_typ4 { get; set; }
        public string chg_typ5 { get; set; }
        public string WEB { get; set; }
        public Nullable<System.DateTime> estRdmDate { get; set; }
        public Nullable<int> age { get; set; }
        public Nullable<bool> PaidYN { get; set; }
        public Nullable<System.DateTime> IssuedDate { get; set; }
        public string CertHist { get; set; }
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
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> LastUpdate { get; set; }
    }
}