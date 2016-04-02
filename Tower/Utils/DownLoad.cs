using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.IO;
using Tower.Models;
using System.Web.UI;
using Tower.ViewModels;

namespace Tower.Utils
{
    public class DownLoad
    {
        TowerEntities db = new TowerEntities();
        StringBuilder sb = new StringBuilder();
        public string csv(string sCounty, string sAction, string issdt, string mwhere, string mwhere1, string mvalue, string mvalue1, string JVvalue)
        {
            if (sAction == null)
            { return "No Nulls Allowed!"; }

            //HttpContext.Current.Server.ScriptTimeout = 300;
            try
            {
                string qparams = "";
                string qparam1 = "";
                string qparam2 = "";
                string qissdt = "";
                string qjvval = "";
                string rowHeaders = "";
                //var dbdata = (from d in db.data where d.County == "" select d).ToList();
                var columnNames = typeof(datatemplate).GetProperties().Select(a => a.Name).ToList();
                //string mycols = "ID, County, action, AccountNumber, ParcelNumber, Tax_yr, certnum, Issued_Date, Face_Amount, Interest_Rate, Cert_Buyer, Account_Status, Cert_Status, Deed_Status, Date_Redeemed_Cert, TDA, TDA_TAX_YR, Application_Date, Applicating_Cert, DeedAppNumber, TDA_Deed_Status, Applicant, Deed_Sale_Date, Sold_To_Applicant, TDA_Date_Redeemed, Roll_Yr, ASMNT_YR, DOR_UC, JV, JV_HMSTD, LND_SQFOOT, DT_LAST_IN, IMP_QUAL, EFF_YR_BLT, TOT_LVG_AR, MKT_AR, NBRHD_CD, TWN, RNG, SEC, CENSUS_BK, PHY_ADDR1, PHY_ZIPCD, ALT_KEY, JV_Range, Homestead, Year_Built, Neighborhood_Census, DOR_Desc, RedeemCount, UnredeemCount, MarketVal, HomesteadType, ExclusionCode, PricingGroup, luTitle,age";
                sb.AppendLine(string.Join(",", columnNames));
                DataTable dt = new DataTable();
                foreach (var mrow in columnNames)
                {
                    dt.Columns.Add(mrow);
                    rowHeaders = rowHeaders + "rowObj." + mrow + ",";

                }
                rowHeaders = rowHeaders.Remove(rowHeaders.Length - 1);

                DataRow row = null;
                //DataTable dbdata = new DataTable();
                //'where 'No Buy'' and CE.[county]=''Lee'' and luTitle=''Single Family Home'' and ExclusionCode=''105'''
                if (sAction == "All")
                {
                    sAction = "Buy','No Buy";
                }

                if (issdt != null) { qissdt = " and issued_date='" + issdt + "'"; }
                if (mwhere != null) { qparam1 = " and " + mwhere + "= '" + mvalue + "' "; }
                if (mwhere1 != null) { qparam2 = " and " + mwhere1 + "= '" + mvalue1 + "' "; }
                if (JVvalue != null) { qjvval = " and JV_Range='" + JVvalue + "' "; }
                qparams = "WHERE Fin_action in ('" + sAction + "') and CE.[county]='" + sCounty + "' " + qissdt + qparam1 + qparam2 + qjvval;
                //var spdbdata = db.spdataDownLoad(sCounty, sAction, issdt, mwhere,mwhere1, mvalue,mvalue1, JVvalue);
                //var spdbdata = db.spdataDownLoad(sCounty, sAction, issdt, mwhere, mvalue, JVvalue, mwhere1, mvalue1);

                var spdbdata = db.spdataDownLoad(qparams);
                //string chk_path = "\\Errors\\error.txt";
                //string filePath = System.Web.HttpContext.Current.Server.MapPath(chk_path);


                var dbdata = (from d in db.datatemplates select d).ToList();

                foreach (var rowObj in dbdata)
                {
                    row = dt.NewRow();
                    //dt.Rows.Add(rowHeaders);
                    dt.Rows.Add(rowObj.ID, rowObj.County, rowObj.Fin_action, rowObj.AccountNumber, rowObj.ParcelNumber, rowObj.Tax_yr, rowObj.certnum, rowObj.Issued_Date, rowObj.Face_Amount, rowObj.Interest_Rate, rowObj.Cert_Buyer, rowObj.Account_Status, rowObj.Cert_Status, rowObj.Deed_Status, rowObj.Date_Redeemed_Cert, rowObj.TDA, rowObj.TDA_TAX_YR, rowObj.Application_Date, rowObj.Applicating_Cert, rowObj.DeedAppNumber, rowObj.TDA_Deed_Status, rowObj.Applicant, rowObj.Deed_Sale_Date, rowObj.Sold_To_Applicant, rowObj.TDA_Date_Redeemed, rowObj.Roll_Yr, rowObj.ASMNT_YR, rowObj.DOR_UC, rowObj.JV, rowObj.JV_HMSTD, rowObj.LND_SQFOOT, rowObj.DT_LAST_IN, rowObj.IMP_QUAL, rowObj.EFF_YR_BLT, rowObj.TOT_LVG_AR, rowObj.MKT_AR, rowObj.NBRHD_CD, rowObj.TWN, rowObj.RNG, rowObj.SEC, rowObj.CENSUS_BK, rowObj.PHY_ADDR1, rowObj.PHY_ZIPCD, rowObj.ALT_KEY, rowObj.JV_Range, rowObj.Homestead, rowObj.Year_Built, rowObj.Neighborhood_Census, rowObj.DOR_Desc, rowObj.RedeemCount, rowObj.UnredeemCount, rowObj.MarketVal, rowObj.HomesteadType, rowObj.ExclusionCode, rowObj.PricingGroup, rowObj.luTitle, rowObj.age, rowObj.orig_action);


                }

                foreach (DataRow drow in dt.Rows)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        string cfix = drow[i].ToString().Replace(",", " ");
                        if (i == dt.Columns.Count - 1)
                        {
                            sb.Append(cfix);
                        }
                        else
                        {
                            sb.Append(cfix + ",");


                        }
                    }

                    sb.Append(Environment.NewLine);
                }

                //using (StreamWriter writer = new StreamWriter(filePath, true))
                //{
                //    writer.WriteLine("Data Full " +
                //       "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                //    writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                //                  }

                sb.Replace("ID", "id");
                string path = System.Web.Hosting.HostingEnvironment.MapPath("~/Drop/");

                File.WriteAllText(path + "test.csv", sb.ToString());
                //using (StreamWriter writer = new StreamWriter(filePath, true))
                //{
                //    writer.WriteLine("Wrote File " +
                //       "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                //    writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);


                //}
                System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
                response.AddHeader("Content-Disposition", "attachment; filename=test.csv");
                response.ContentType = "text/csv";
                response.TransmitFile(path + "test.csv");
                response.End();

                //using (StreamWriter writer = new StreamWriter(filePath, true))
                //{
                //    writer.WriteLine("Returned File " +
                //       "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                //    writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);


                //}

            }
            catch (Exception ex)
            {
                ErrorHandler e = new ErrorHandler();

                e.ErrorLog(ex.InnerException);
            }

            //return dt.ToString();
            return "Sucess";
        }


        public string njexcel(string sTownship)
        {
            if (sTownship != null)
            {
                try
                {
                    var grid = new System.Web.UI.WebControls.GridView();

                     var msubs = from k in db.certDatas
                                 group k by new { k.APN, k.LastUpdate.Value.Month, k.LastUpdate.Value.Year, k.IssuedDate } into g           
                      select new {
                          APN=g.Key.APN,
                          max= g.Max(t=>t.Subsequent),
                          mth=g.Key.Month,
                          ldate=g.Max(x=>x.LastUpdate),
                          yr = g.Key.Year,
                          IssDt = g.Key.IssuedDate

                      };
                     var dbdata = (from g in
                              (from jd in db.JerseyDatas
                               join m in msubs
                               //on jd.APN equals m.APN into mj
                               on new { Key1 = jd.APN, Key2 = jd.IssuedDate } equals new { Key1 = m.APN, Key2 = m.IssDt } into mj
                               from j in mj.DefaultIfEmpty()
                               let Updated_Sub_10_15 = j.mth == 10 && j.yr == 2015 ? j.max : 0
                               let Updated_Sub_11_15 = j.mth == 11 && j.yr == 2015 ? j.max : 0
                               let Updated_Sub_12_15 = j.mth == 12 && j.yr == 2015 ? j.max : 0
                               let Sub_01_16 = j.mth == 1 && j.yr == 2016 ? j.max : 0
                               let Sub_02_16 = j.mth == 2 && j.yr == 2016 ? j.max : 0
                               let ldate = j.ldate
                               where (jd.Township == sTownship && jd.CertificateNumber != null)
                               select new
                               {
                                   ldate,
                                   Updated_Sub_10_15,
                                   Updated_Sub_11_15,
                                   Updated_Sub_12_15,
                                   Sub_01_16,
                                   Sub_02_16,
                                   jd.Sort_No,
                                   jd.APN,
                                   jd.Township,
                                   jd.County,
                                   jd.Block,
                                   jd.Lot,
                                   jd.Qualifier,
                                   jd.Additional_Lots,
                                   jd.Amount,
                                   jd.Type,
                                   jd.Notes,
                                   jd.HouseNo,
                                   jd.DirL,
                                   jd.StreetName,
                                   jd.StreetSufx,
                                   jd.DirR,
                                   jd.Unit,
                                   jd.City,
                                   jd.State,
                                   jd.ZIP,
                                   jd.FullLocation,
                                   jd.CityStateZip,
                                   jd.Owner,
                                   jd.AbsentOwner,
                                   jd.Class,
                                   jd.LandUse,
                                   jd.Zoning,
                                   jd.BldgDescr,
                                   jd.NoStories,
                                   jd.NoUnits,
                                   jd.YearBuilt,
                                   jd.LotWidth,
                                   jd.LotDepth,
                                   jd.LotSize,
                                   jd.Longitude,
                                   jd.Latitude,
                                   jd.SalePrice,
                                   jd.SaleDate,
                                   jd.PriorSalePrice,
                                   jd.PriorSaleDate,
                                   jd.AsmntYear,
                                   jd.LandValue,
                                   jd.ImpValue,
                                   jd.TotalValue,
                                   jd.TaxYear,
                                   jd.Taxes,
                                   jd.C1stMtgAmt,
                                   jd.C1stMtgRate,
                                   jd.C1stMtgTerm,
                                   jd.C1stMtgType,
                                   jd.C2ndMtgAmt,
                                   jd.C2ndMtgRate,
                                   jd.C2ndMtgTerm,
                                   jd.C2ndMtgType,
                                   jd.MailOwner,
                                   jd.MailCO,
                                   jd.MailAddress,
                                   jd.MailCityState,
                                   jd.MailZip,
                                   jd.OwnerPhone,
                                   jd.Owner_Name_Address_,
                                   jd.CertificateNumber,
                                   jd.SaleDateWeb,
                                   jd.LienHolder,
                                   jd.SaleAmount,
                                   jd.chg_typ1,
                                   jd.YearInSale,
                                   jd.Subsequent,
                                   jd.chg_typ2,
                                   jd.chg_typ3,
                                   jd.chg_typ4,
                                   jd.chg_typ5,
                                   jd.WEB,
                                   jd.estRdmDate,
                                   jd.age,
                                   jd.PaidYN,
                                   jd.IssuedDate,
                                   jd.CertHist
                               }) // new {Updated_Sub_11_15, Updated_Sub_10_15, jd.APN, jd.CertificateNumber, jd.Township, jd.Amount, jd.LienHolder, jd.chg_typ1, jd.chg_typ2, jd.chg_typ3, jd.chg_typ4, jd.chg_typ5, jd.Subsequent, jd.PaidYN, jd.IssuedDate, jd.estRdmDate, jd.age, jd.YearInSale, jd.WEB })
                          group g by  new { g.Sort_No,	g.APN,	g.Township,	g.County,	g.Block,	g.Lot,	g.Qualifier,	g.Additional_Lots,	g.Amount,	g.Type,	g.Notes,	g.HouseNo,	g.DirL,	g.StreetName,	g.StreetSufx,	g.DirR,	g.Unit,	g.City,	g.State,	g.ZIP,	g.FullLocation,	g.CityStateZip,	g.Owner,	g.AbsentOwner,	g.Class,	g.LandUse,	g.Zoning,	g.BldgDescr,	g.NoStories,	g.NoUnits,	g.YearBuilt,	g.LotWidth,	g.LotDepth,	g.LotSize,	g.Longitude,	g.Latitude,	g.SalePrice,	g.SaleDate,	g.PriorSalePrice,	g.PriorSaleDate,	g.AsmntYear,	g.LandValue,	g.ImpValue,	g.TotalValue,	g.TaxYear,	g.Taxes,	g.C1stMtgAmt,	g.C1stMtgRate,	g.C1stMtgTerm,	g.C1stMtgType,	g.C2ndMtgAmt,	g.C2ndMtgRate,	g.C2ndMtgTerm,	g.C2ndMtgType,	g.MailOwner,	g.MailCO,	g.MailAddress,	g.MailCityState,	g.MailZip,	g.OwnerPhone,	g.Owner_Name_Address_,	g.CertificateNumber,	g.SaleDateWeb,	g.LienHolder,	g.SaleAmount,	g.chg_typ1,	g.YearInSale,	g.Subsequent,	g.chg_typ2,	g.chg_typ3,	g.chg_typ4,	g.chg_typ5,	g.WEB,	g.estRdmDate,	g.age,	g.PaidYN,	g.IssuedDate,	g.CertHist } into p
                          select new NJDownloadView
                            {
                               Sort_No = p.Key.Sort_No,
                               APN = p.Key.APN,
                               Township = p.Key.Township,
                               County = p.Key.County,
                               Block = p.Key.Block,
                               Lot = p.Key.Lot,
                               Qualifier = p.Key.Qualifier,
                               Additional_Lots = p.Key.Additional_Lots,
                               Amount = p.Key.Amount,
                               Type = p.Key.Type,
                               Notes = p.Key.Notes,
                               HouseNo = p.Key.HouseNo,
                               DirL = p.Key.DirL,
                               StreetName = p.Key.StreetName,
                               StreetSufx = p.Key.StreetSufx,
                               DirR = p.Key.DirR,
                               Unit = p.Key.Unit,
                               City = p.Key.City,
                               State = p.Key.State,
                               ZIP = p.Key.ZIP,
                               FullLocation = p.Key.FullLocation,
                               CityStateZip = p.Key.CityStateZip,
                               Owner = p.Key.Owner,
                               AbsentOwner = p.Key.AbsentOwner,
                               Class = p.Key.Class,
                               LandUse = p.Key.LandUse,
                               Zoning = p.Key.Zoning,
                               BldgDescr = p.Key.BldgDescr,
                               NoStories = p.Key.NoStories,
                               NoUnits = p.Key.NoUnits,
                               YearBuilt = p.Key.YearBuilt,
                               LotWidth = p.Key.LotWidth,
                               LotDepth = p.Key.LotDepth,
                               LotSize = p.Key.LotSize,
                               Longitude = p.Key.Longitude,
                               Latitude = p.Key.Latitude,
                               SalePrice = p.Key.SalePrice,
                               SaleDate = p.Key.SaleDate,
                               PriorSalePrice = p.Key.PriorSalePrice,
                               PriorSaleDate = p.Key.PriorSaleDate,
                               AsmntYear = p.Key.AsmntYear,
                               LandValue = p.Key.LandValue,
                               ImpValue = p.Key.ImpValue,
                               TotalValue = p.Key.TotalValue,
                               TaxYear = p.Key.TaxYear,
                               Taxes = p.Key.Taxes,
                               C1stMtgAmt = p.Key.C1stMtgAmt,
                               C1stMtgRate = p.Key.C1stMtgRate,
                               C1stMtgTerm = p.Key.C1stMtgTerm,
                               C1stMtgType = p.Key.C1stMtgType,
                               C2ndMtgAmt = p.Key.C2ndMtgAmt,
                               C2ndMtgRate = p.Key.C2ndMtgRate,
                               C2ndMtgTerm = p.Key.C2ndMtgTerm,
                               C2ndMtgType = p.Key.C2ndMtgType,
                               MailOwner = p.Key.MailOwner,
                               MailCO = p.Key.MailCO,
                               MailAddress = p.Key.MailAddress,
                               MailCityState = p.Key.MailCityState,
                               MailZip = p.Key.MailZip,
                               OwnerPhone = p.Key.OwnerPhone,
                               Owner_Name_Address_ = p.Key.Owner_Name_Address_,
                               CertificateNumber = p.Key.CertificateNumber,
                               SaleDateWeb = p.Key.SaleDateWeb,
                               LienHolder = p.Key.LienHolder,
                               SaleAmount = p.Key.SaleAmount,
                               chg_typ1 = p.Key.chg_typ1,
                               YearInSale = p.Key.YearInSale,
                               Subsequent = p.Key.Subsequent,
                               chg_typ2 = p.Key.chg_typ2,
                               chg_typ3 = p.Key.chg_typ3,
                               chg_typ4 = p.Key.chg_typ4,
                               chg_typ5 = p.Key.chg_typ5,
                               WEB = p.Key.WEB,
                               estRdmDate = p.Key.estRdmDate,
                               age = p.Key.age,
                               PaidYN = p.Key.PaidYN,
                               IssuedDate = p.Key.IssuedDate,
                               CertHist = p.Key.CertHist,
                               UPDSubs10 = p.Max(x => x.Updated_Sub_10_15),
                               UPDSubs11 = p.Max(x => x.Updated_Sub_11_15),
                               UPDSubs12 = p.Max(x => x.Updated_Sub_12_15),
                               UPDSubs0116 =p.Max(x => x.Sub_01_16),
                               UPDSubs0216 =  p.Max(x => x.Sub_02_16),
                               LastUpdate = p.Max(x=> x.ldate)                             

                           }).ToList();

                  

                    grid.DataSource = dbdata;
                    grid.DataBind();
                    System.Web.HttpResponse Response = System.Web.HttpContext.Current.Response;
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", "attachment; filename=" + sTownship + ".xls");
                    Response.ContentType = "application/excel";
                    StringWriter sw = new StringWriter();
                    HtmlTextWriter htw = new HtmlTextWriter(sw);
                    grid.RenderControl(htw);
                    Response.Write(sw.ToString());
                    Response.End();
                    return "Sucess";
                }
                catch (Exception e)
                {
                    ErrorHandler ex = new ErrorHandler();
                    ex.ErrorLog(e);
                    return "Fail";

                }
            }
            else
            {
                ErrorHandler ex = new ErrorHandler();
                ex.ErrrorText("No Township Selected!");
                return "Fail";
            }

        }
    }
}