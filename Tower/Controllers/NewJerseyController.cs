using System;
//using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Tower.Models;
using Tower.ViewModels;
using System.Text.RegularExpressions;
using Tower.Utils;
using System.Data.OleDb;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;



namespace Tower.Controllers
{
    [Authorize(Roles = "Admin")]
    public class NewJerseyController : Controller
    {
        private TowerEntities db = new TowerEntities();
        private static object Lock = new object();

        // GET: NewJersey
        public ActionResult Index()
        {

        //    List<SelectListItem> _months=  db.JerseyDatas.OrderBy(d=>d.IssuedDate)
        //        .Select(d=>d.IssuedDate)
        //        .AsEnumerable().Select(date => date.ToString("MM-dd-yyyy")).Distinct()
        //         .Select(formattedDate => new SelectListItem { Text = formattedDate, Value = formattedDate })
        //.ToList(); 

            //County
            var countyname = db.JerseyDatas.Select(c => new { c.Township,c.IssuedDate }).Distinct().OrderBy(x => x.IssuedDate).AsEnumerable().Select(x => new SelectListItem

           
                {
                   
                    Text = x.Township + " - " + x.IssuedDate.GetValueOrDefault().ToString("MM/dd/yyyy"),
                    Value = x.Township


                }).ToList();

          
            var ddlist = new NJReportView
            {
                qtown = countyname// items
            };
            


            return View(ddlist);
        }

         [HttpPost]
        public ActionResult index([Bind(Include = "Township,qtown")] NJReportView  repview)
        {

            string sTownship = repview.Township;
            if (sTownship != null)
             {
                 lock (Lock)
                 {
                     try
                     {
                         DownLoad csvexport = new DownLoad();
                         string checkStat = csvexport.njexcel(sTownship);

                         if (checkStat == "Sucesss")
                         { }
                         else { throw new Exception("Download Failed!"); }
                     }
                     catch (Exception e)
                     {
                         ErrorHandler ex = new ErrorHandler();

                         ex.ErrorLog(e.InnerException);


                     }
                 }

             }
             
             //County
            var countyname = db.JerseyDatas.Select(c => new { c.Township }).Distinct().OrderBy(x => x.Township).Select(x => new SelectListItem
                       
            {
                Text = x.Township,
                Value = x.Township

            }).ToList();
                        
            var ddlist = new NJReportView
            {
                qtown = countyname
            };

            return View("index");


        }
        [HttpGet]
        public ActionResult _results(string selectedID)
        {

            var msubs = from k in db.certDatas
                        group k by new { k.APN, k.LastUpdate.Value.Month, k.LastUpdate.Value.Year,k.IssuedDate } into g           
                      select new {
                          APN=g.Key.APN,
                          max= g.Max(t=>t.Subsequent),
                          mth=g.Key.Month,
                          yr=g.Key.Year,
                          IssDt=g.Key.IssuedDate

                      };

            //let myage = (p.Date_Redeemed_Cert != null) ? ((p.Date_Redeemed_Cert.Value.Year - p.Issued_Date.Value.Year) * 12) + ((p.Date_Redeemed_Cert.Value.Month - p.Issued_Date.Value.Month)) : 0
            var repvals = from g in
                              (from jd in db.JerseyDatas
                               join m in msubs
                               //on jd.APN equals m.APN into mj
                               on new { Key1 = jd.APN, Key2 = jd.IssuedDate } equals new { Key1 = m.APN, Key2 = m.IssDt } into mj
                               from j in mj.DefaultIfEmpty()
                               let Sub_10_15 = j.mth == 10 && j.yr ==2015 ? j.max : 0
                               let Sub_11_15 = j.mth == 11 && j.yr == 2015 ? j.max : 0
                               let Sub_12_15 = j.mth == 12 && j.yr == 2015 ? j.max : 0
                               let Sub_01_16 = j.mth == 1 && j.yr == 2016 ? j.max : 0
                               let Sub_02_16 = j.mth == 2 && j.yr == 2016 ? j.max : 0
                               let Sub_03_16 = j.mth == 3 && j.yr == 2016 ? j.max : 0
                               where (jd.Township == selectedID && jd.CertificateNumber != null)
                               select new { Sub_12_15, Sub_11_15, Sub_10_15, Sub_01_16, Sub_02_16, Sub_03_16, jd.subMthYr, jd.APN, jd.CertificateNumber, jd.Township, jd.Amount, jd.LienHolder, jd.chg_typ1, jd.chg_typ2, jd.chg_typ3, jd.chg_typ4, jd.chg_typ5, jd.Subsequent, jd.PaidYN, jd.IssuedDate, jd.estRdmDate, jd.age, jd.YearInSale, jd.WEB })
                          group g by new {g.subMthYr, g.APN, g.CertificateNumber, g.Township, g.Amount, g.LienHolder, g.chg_typ1, g.chg_typ2, g.chg_typ3, g.chg_typ4, g.chg_typ5, g.Subsequent, g.PaidYN, g.IssuedDate, g.estRdmDate, g.age, g.YearInSale, g.WEB } into p
                          select new NJReportView
                          {

                              APN = p.Key.APN,
                              CertificateNumber = p.Key.CertificateNumber,
                              Township = p.Key.Township,
                              Amount = p.Key.Amount,
                              LienHolder = p.Key.LienHolder,
                              chg_typ1 = p.Key.chg_typ1,
                              chg_typ2 = p.Key.chg_typ2,
                              chg_typ3 = p.Key.chg_typ3,
                              chg_typ4 = p.Key.chg_typ4 ,
                              chg_typ5 = p.Key.chg_typ5,
                              Subsequent = p.Key.Subsequent,
                              UPDSubs10 = p.Key.subMthYr >= 1510 ? null : p.Max(x => x.Sub_10_15),
                              UPDSubs11 = p.Key.subMthYr >= 1511 ? null : p.Max(x => x.Sub_11_15),
                              UPDSubs12 = p.Key.subMthYr >= 1512 ? null : p.Max(x => x.Sub_12_15),
                              UPDSubs0116 = p.Key.subMthYr >= 1601 ? null : p.Max(x => x.Sub_01_16),
                              UPDSubs0216 = p.Key.subMthYr >= 1602 ? null : p.Max(x => x.Sub_02_16),
                              UPDSubs0316 = p.Key.subMthYr >= 1603 ? null : p.Max(x => x.Sub_03_16),
                              //UPDSubs10 = p.Key.IssuedDate.Value.Year==2015 && p.Key.IssuedDate.Value.Month >= 10  ? null : p.Max(x => x.Sub_10_15),
                              //UPDSubs11 = p.Key.IssuedDate.Value.Year == 2015 && p.Key.IssuedDate.Value.Month >= 11 ? null : p.Max(x => x.Sub_11_15),
                              //UPDSubs12 = p.Key.IssuedDate.Value.Year == 2015 && p.Key.IssuedDate.Value.Month >= 12 ? null : p.Max(x => x.Sub_12_15),
                              //UPDSubs0116 = p.Key.IssuedDate.Value.Year == 2016 ? null : p.Max(x => x.Sub_01_16),
                              //UPDSubs11 = p.Max(x => x.Sub_11_15),
                              //UPDSubs12 = p.Max(x => x.Sub_12_15),
                              PaidYN = p.Key.PaidYN,
                              IssuedDate = p.Key.IssuedDate,
                              estRdmDate = p.Key.estRdmDate,
                              age = p.Key.age,
                              YearInSale = p.Key.YearInSale,
                              WEB = p.Key.WEB

                          };

            repvals = repvals.OrderBy(x => x.CertificateNumber);

            //ChatHub stmessage = new ChatHub();
            //stmessage.Send(HttpContext.User.Identity.Name.ToString(), selectedID);
            //var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            //context.Clients.All.Send(HttpContext.User.Identity.Name.ToString(), selectedID);

            return PartialView("_results",repvals.ToList());
        }
        // GET: NewJersey/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JerseyData jerseyData = db.JerseyDatas.Find(id);
            if (jerseyData == null)
            {
                return HttpNotFound();
            }
            return View(jerseyData);
        }

        public ActionResult updateSubs()
        {
            return View();
        }

        [HttpPost]
        public ActionResult updateSubs(HttpPostedFileBase file)
        {
            var path = "";
            //check file

            if (file != null && file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                path = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/Drop/"), fileName);
                file.SaveAs(path);
            }
            else
            {
                ViewBag.Message = "File is Empty?";
                return View();

            }
            //Check extension
            string xchk = Path.GetExtension(file.FileName);
            if (xchk == ".xls" | xchk == ".xlsx")
            
            {
               
            }
            else
            {
                ViewBag.Message = "Must be an Excel File!";
                return View();

            }


          
                    string excelConnectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=Excel 8.0", path);
                    //string excelConnectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=Excel 8.0", path); 
 try{
                    // Create Connection to Excel Workbook 
                    using (OleDbConnection connection = 
                                 new OleDbConnection(excelConnectionString)) 
                    { 
                       
 
                        connection.Open();
                        var dtSchema = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                    string Sheet1 = dtSchema.Rows[0].Field<string>("TABLE_NAME");

                        OleDbCommand command = new OleDbCommand
                               ("Select * FROM [" + Sheet1 + "]", connection); 
                        // Create DbDataReader to Data Worksheet 
                        using (DbDataReader dr = command.ExecuteReader()) 
                        { 
 
                            // SQL Server Connection String 
                            SqlConnection sqlCon = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
                            sqlCon.Open();
                            // Bulk Copy to SQL Server 
                            using (SqlBulkCopy bulkCopy = 
                                       new SqlBulkCopy(sqlCon)) 
                            { 
                                bulkCopy.DestinationTableName = "NJSubs";

                                SqlBulkCopyColumnMapping mapID =
                   new SqlBulkCopyColumnMapping("APN", "APN");
                                bulkCopy.ColumnMappings.Add(mapID);

                                SqlBulkCopyColumnMapping mapName =
                                    new SqlBulkCopyColumnMapping("Township", "Township");
                                bulkCopy.ColumnMappings.Add(mapName);

                                SqlBulkCopyColumnMapping mapMumber =
                                    new SqlBulkCopyColumnMapping("Certnum", "Certnum");
                                bulkCopy.ColumnMappings.Add(mapMumber);

                                SqlBulkCopyColumnMapping issDt =
                   new SqlBulkCopyColumnMapping("IssuedDate", "IssuedDate");
                                bulkCopy.ColumnMappings.Add(issDt);

                                SqlBulkCopyColumnMapping subAmt =
                                    new SqlBulkCopyColumnMapping("Subsequent", "Subsequent");
                                bulkCopy.ColumnMappings.Add(subAmt);

                                SqlBulkCopyColumnMapping lastUpdt =
                                    new SqlBulkCopyColumnMapping("LastUpdate", "LastUpdate");
                                bulkCopy.ColumnMappings.Add(lastUpdt);



                                bulkCopy.WriteToServer(dr); 
                                
                            } 
                        }
                        //Close connection
                        connection.Close();

                        lock (Lock)
                        {
                            try
                            {

                                var subsUpdt = db.spSubsequentUpdate("Dominick");
                            }
                            catch (DataException e)
                            {
                                ErrorHandler ex = new ErrorHandler();

                                ex.ErrorLog(e.InnerException);
                                ViewBag.Message = "That didn't work!  " + e.Message;
                                //return View();
                            }
                        }

                        //Delete file when done
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                        
                        //Check that the update hasn't already been done. Compare NJSubs with CertData. If done return appropriate response. 


                    }
     ViewBag.Message="You have sucessfully updated the Database!" ;

 }
          catch (Exception e)
                     {
                         ErrorHandler ex = new ErrorHandler();

                         ex.ErrorLog(e.InnerException);
              ViewBag.Message="That didn't work!  " + e.Message;

                     }


         return View();
                } 
 
             


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
