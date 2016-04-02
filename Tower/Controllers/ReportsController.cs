using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using Tower.Models;
using Tower.Utils;
using Tower.ViewModels;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data.Entity.Validation;
using System.Reflection;
using System.Linq.Expressions;



namespace Tower.Controllers
{
    [Authorize(Roles = "Admin")]
    [HandleError]
    public class ReportsController : Controller
    {
        private TowerEntities db = new TowerEntities();
        private static object Lock = new object();

        // GET: Reports
        public ActionResult Index()
        {

              try
                {
                    //County
                    var countyname = db.Certificates.Select(c => c.County).Distinct();
                    List<SelectListItem> items = new List<SelectListItem>();
                    foreach (var t in countyname)
                    {
                        SelectListItem s = new SelectListItem();
                        s.Text = t.ToString();
                        s.Value = t.ToString();
                        items.Add(s);
                    }

                    //action
                    var bnbuy = db.Certificates.Select(c => c.Fin_action).Distinct();
                    List<SelectListItem> bitems = new List<SelectListItem>();
                    foreach (var t in bnbuy)
                    {
                        SelectListItem s = new SelectListItem();
                        s.Text = t.ToString();
                        s.Value = t.ToString();
                        bitems.Add(s);
                    }
                    SelectListItem n = new SelectListItem();
                    n.Text = "All";
                    n.Value = "All";
                    bitems.Add(n);
                  
                  //Issue Date
                   var issDt = db.Certificates.Select(c => c.Issued_Date).Distinct();
                    List<SelectListItem> ditems = new List<SelectListItem>();
                    foreach (var t in issDt)
                    {
                        SelectListItem s = new SelectListItem();
                        s.Text = Convert.ToDateTime(t).ToString("MM/dd/yyyy").Trim();
                        s.Value = Convert.ToDateTime(t).ToString("MM/dd/yyyy").Trim();
                        ditems.Add(s);
                    }
                  //Columns
                    var colnames = db.ColNames.Select(c => c.DDItems).Distinct();
                    List<SelectListItem> qitems = new List<SelectListItem>();
                  foreach (var t in colnames)
                  {
                      SelectListItem s = new SelectListItem();
                      s.Text = t.ToString().Trim();
                      s.Value = t.ToString().Trim();
                      qitems.Add(s);

                  }
                  var dropitems = new selectDropDown
                  {
                      qaction = bitems,
                      qcounty = items,
                      param1 = qitems,
                      param2 = qitems,
                      issdt = ditems

                  };
                  return View(dropitems);
                }
              catch (Exception e)
              {
                  ErrorHandler ex = new ErrorHandler();
                  ex.ErrorLog(e);
                  Redirect("Error");

              }

              return View();
        }



        [HttpPost]
        public ActionResult index([Bind(Include = "action,county,qparam1,qparam2,paction,pissdt")] selectDropDown repvals)  
        {
            string qgroup = "";
            string qwhere = "";
            string username = HttpContext.User.Identity.Name.ToString();
            if (ModelState.IsValid)
            {

                //ReportVal ReportVal = db.ReportVals.Find(ID);
                var chkuser = (from p in db.ReportParams where p.nuser == username select p);
                foreach (var detail in chkuser)
                {
                    db.ReportParams.Remove(detail);
                }

                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {

                    ErrorHandler ex = new ErrorHandler();
                    ex.ErrorLog(e);
                }
                try
                {
                    ReportParam RVals = new ReportParam();
                    RVals.Fin_action = repvals.paction;
                    RVals.Issued_Date = repvals.pissdt;
                    RVals.County = repvals.county;
                    RVals.gparam1 = repvals.qparam1;
                    RVals.gparam2 = repvals.qparam2;
                    RVals.nuser = username;
                    db.ReportParams.Add(RVals);
                    db.SaveChanges();


                }
                catch (DbEntityValidationException e)
                {
                    ErrorHandler ex = new ErrorHandler();
                    foreach (var item in e.EntityValidationErrors)
                    {
                        ex.ErrorLog(e);
                    }


                }
               
                string p3 = null;
                string mcounty = repvals.county;
                string maction = repvals.paction;
                string p1 = repvals.qparam1;  // "Tax Yr";
                string p2 = repvals.qparam2;
                string pselect = "";
                string pselect2 = "";
                int qswitch = 0;
                if(maction=="All")
                {
                    maction = "Buy','No Buy";
                    pselect = "County,'All' as Fin_action,";
                    qgroup = "County,";
                    pselect2 = "County,Fin_action,";

                }
                else
                {
                    pselect = "County,Fin_action,";
                    qgroup = "County,Fin_action,";
                    pselect2 = "County,Fin_action,";
                }
               

                if (repvals.qparam2 != null)
                {
                    //pselect = "County,Fin_action," + p1 + "," + repvals.qparam2;
                    //qgroup = "County,Fin_action," + p1 + "," + repvals.qparam2;
                    pselect = pselect + p1 + "," + repvals.qparam2;
                    pselect2 = pselect2 + p1 + "," + repvals.qparam2;
                    qgroup = qgroup + p1 + "," + repvals.qparam2;
                    
                }
                else
                {
                    //pselect = "County,Fin_action," + p1;
                    //qgroup = "County,Fin_action," + p1;
                    pselect = pselect + p1;
                    pselect2 = pselect2 + p1;
                    qgroup = qgroup + p1;
                    qswitch = 1;
                }


                if (repvals.pissdt != "Select Issue Date")
                {
                    p3 = " and issued_date = '" + repvals.pissdt + "' ";
                }
                qwhere = "County = '"  + mcounty + "' and Fin_action in('" + maction + "') " + p3;
                lock (Lock)
                {
                    try
                    {

                        var repgen = db.spCounty(qgroup, qwhere, qswitch, username, pselect,pselect2);
                    }
                    catch (DataException e)
                    {
                        ErrorHandler ex = new ErrorHandler();

                        ex.ErrorLog(e.InnerException);

                    }
                }
            }
            else
            {
                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        string err = error.ToString();
                    }
                }
                try
                {
                    //County
                    var countyname = db.Certificates.Select(c => c.County).Distinct();
                    List<SelectListItem> items = new List<SelectListItem>();
                    foreach (var t in countyname)
                    {
                        SelectListItem s = new SelectListItem();
                        s.Text = t.ToString();
                        s.Value = t.ToString();
                        items.Add(s);
                    }

                    //action
                    var bnbuy = db.Certificates.Select(c => c.Fin_action).Distinct();
                    List<SelectListItem> bitems = new List<SelectListItem>();
                    foreach (var t in bnbuy)
                    {
                        SelectListItem s = new SelectListItem();
                        s.Text = t.ToString();
                        s.Value = t.ToString();
                        bitems.Add(s);
                    }
                    SelectListItem n = new SelectListItem();
                    n.Text = "All";
                    n.Value = "All";
                    bitems.Add(n);
                    //Issue Date
                    var issDt = db.Certificates.Select(c => c.Issued_Date).Distinct();
                    List<SelectListItem> ditems = new List<SelectListItem>();
                    foreach (var t in issDt)
                    {
                        SelectListItem s = new SelectListItem();
                        s.Text = Convert.ToDateTime(t).ToString("MM/dd/yyyy").Trim();
                        s.Value = Convert.ToDateTime(t).ToString("MM/dd/yyyy").Trim();
                        ditems.Add(s);
                    }
                    //Columns
                    var colnames = db.ColNames.Select(c => c.DDItems).Distinct();
                    List<SelectListItem> qitems = new List<SelectListItem>();
                    foreach (var t in colnames)
                    {
                        SelectListItem s = new SelectListItem();
                        s.Text = t.ToString().Trim();
                        s.Value = t.ToString().Trim();
                        qitems.Add(s);

                    }
                    var dropitems = new selectDropDown
                    {
                        qaction = bitems,
                        qcounty = items,
                        param1 = qitems,
                        param2 = qitems,
                        issdt = ditems
                        //qaction = bitems,
                        //qcounty = items,
                        //Cert = qitems,
                        //Account = qitems,
                        //pchoice3 = ditems

                    };

                                      
                    return View(dropitems);
                }
                catch (Exception e)
                {
                    ErrorHandler ex = new ErrorHandler();

                    ex.ErrorLog(e);


                }


            }

            return RedirectToAction("Report"); 
            

        }

        public ActionResult Report(string Command) 
        {

            string cuser =  HttpContext.User.Identity.Name.ToString();

            //Check if empty and return error page
            bool chkview = db.percentCalcs.Any(n => n.nuser.Equals(cuser));
            if (!chkview)
            {

                ErrorHandler ex = new ErrorHandler();
                ex.ErrrorText("No Results for Report: " + cuser);
               
                
                return View("Index");
            }
            var rparams = (from rp in db.ReportParams
                          where rp.nuser == cuser
                          select rp).FirstOrDefault();
            string idate = "";
            if (rparams.Issued_Date != "Select Issue Date") { idate = " -- Issue Date = " + rparams.Issued_Date; }
            ViewBag.MyTitle =  rparams.County +   " -- " + rparams.Fin_action  + idate;         
                            
                            

             

            string sortby = "true";
            string viewstring = "true";
            SortView chksort = new SortView();
            UpdateModel(chksort);
            if (Command == rparams.gparam1)
            {
                Command = "qparam1"; if (chksort.sFace_Value == "true")
                { viewstring = "false"; sortby = "true"; }
                else { viewstring = "true"; sortby = "false"; }
            }
            if (Command == rparams.gparam2)
            {
                Command = "qparam2"; if (chksort.sFace_Value == "true")
                { viewstring = "false"; sortby = "true"; }
                else { viewstring = "true"; sortby = "false"; }
            }
      
            if (Command == "Face Value")
            {
                if (chksort.sFace_Value == "true")
                { viewstring = "false"; sortby = "true"; }
                else { viewstring = "true"; sortby = "false"; }
            }
            if (Command == "Month 6")
            {
                if (chksort.sFace_Value == "true")
                { viewstring = "false"; sortby = "true"; }
                else { viewstring = "true"; sortby = "false"; }
            }
            if (Command == "Month 12")
            {
                if (chksort.sFace_Value == "true")
                { viewstring = "false"; sortby = "true"; }
                else { viewstring = "true"; sortby = "false"; }
            }
            if (Command == "Month 23")
            {
                if (chksort.sFace_Value == "true")
                { viewstring = "false"; sortby = "true"; }
                else { viewstring = "true"; sortby = "false"; }
            }
            if (Command == "Month 30")
            {
                if (chksort.sFace_Value == "true")
                { viewstring = "false"; sortby = "true"; }
                else { viewstring = "true"; sortby = "false"; }
            }
            if (Command == "Month 36")
            {
                if (chksort.sFace_Value == "true")
                { viewstring = "false"; sortby = "true"; }
                else { viewstring = "true"; sortby = "false"; }
            }
            if (Command == "Today")
            {
                if (chksort.sFace_Value == "true")
                { viewstring = "false"; sortby = "true"; }
                else { viewstring = "true"; sortby = "false"; }
            }


            // string cuser=HttpContext.User.Identity.Name.ToString();

            //Check if empty and return error page
            var pview = from a in db.percentCalcs
                        where a.nuser == cuser

                        select new CalcViewModel()
                        {
                            ID = a.ID,
                            action = a.action,
                            PricingGroup = a.PricingGroup,
                            pchoice = a.pchoice,
                            pchoice2 = a.pchoice2,
                            Face_Value = a.Face_Value,
                            Month_6 = a.Month_6,
                            Month_12 = a.Month_12,
                            Month_23 = a.Month_23,
                            Month_30 = a.Month_30,
                            Month_36 = a.Month_36,
                            Today = a.Today,
                            qparam2 = rparams.gparam2, // repvals.qparam2,
                            pissdt = rparams.Issued_Date, // repvals.pissdt,
                            qparam1 = rparams.gparam1,  // repvals.qparam1,
                            county = rparams.County,  // repvals.county,
                            paction = rparams.Fin_action,  // repvals.paction,
                            sFace_Value = viewstring,
                            DDID=rparams.ID
                        };


            switch (Command)
            {
                case "qparam1":
                    if (sortby == "true")
                    { pview = pview.OrderByDescending(s => s.PricingGroup); }
                    else { pview = pview.OrderBy(s => s.PricingGroup); }

                    break;

                case "qparam2":
                    if (sortby == "true")
                    { pview = pview.OrderByDescending(s => s.pchoice); }
                    else { pview = pview.OrderBy(s => s.pchoice); }

                    break;
              

                case "Face Value":
                    if (sortby == "true")
                    { pview = pview.OrderByDescending(s => s.Face_Value); }
                    else { pview = pview.OrderBy(s => s.Face_Value); }

                    break;


                case "Month 6":
                    if (sortby == "true")
                    { pview = pview.OrderByDescending(s => s.Month_6); }
                    else { pview = pview.OrderBy(s => s.Month_6); }

                    break;

                case "Month 12":
                    if (sortby == "true")
                    { pview = pview.OrderByDescending(s => s.Month_12); }
                    else { pview = pview.OrderBy(s => s.Month_12); }

                    break;

                case "Month 23":
                    if (sortby == "true")
                    { pview = pview.OrderByDescending(s => s.Month_23); }
                    else { pview = pview.OrderBy(s => s.Month_23); }

                    break;

                case "Month 30":
                    if (sortby == "true")
                    { pview = pview.OrderByDescending(s => s.Month_30); }
                    else { pview = pview.OrderBy(s => s.Month_30); }

                    break;

                case "Month 36":
                    if (sortby == "true")
                    { pview = pview.OrderByDescending(s => s.Month_36); }
                    else { pview = pview.OrderBy(s => s.Month_36); }

                    break;

                case "Today":
                    if (sortby == "true")
                    { pview = pview.OrderByDescending(s => s.Today); }
                    else { pview = pview.OrderBy(s => s.Today); }

                    break;

                default:

                    pview = pview.OrderBy(s => s.PricingGroup);
                    break;
            }

            return View(pview.ToList());

        }
        //Data Dump

        public ActionResult DownLoaddt(int ID)  
        {
            string username = HttpContext.User.Identity.Name.ToString();
            string mcounty = null;
            string maction = null;
            string W1 =  null;
            string W2 =  null;
            string issdt = null;
            string V1 = null;
            string V2 = null;
            string JV1 = null;


            //Check for INT ID =1
            if (ID == 1)
            {
                //if qparam 1 is null just query fin_actin, county and issued date else send all parameters from reprtParams
                try
                {
                    var rparams = (from rp in db.ReportParams
                                   where rp.nuser == username
                                   select rp).FirstOrDefault();
                    if (rparams.qwparam1 != null)
                    {
                        
                        mcounty = rparams.County;  // pCalc.county;
                        maction = rparams.Fin_action;  // pCalc.action;
                        W1 = rparams.gparam1;  // qwhere1; // pCalc.PricingGroup;
                        W2 = rparams.gparam2;  // pCalc.pchoice;
                        if (rparams.Issued_Date != "Select Issue Date")
                        { issdt = rparams.Issued_Date; }
                        V1 = rparams.qwparam1;
                        V2 = rparams.qwparam2;
                        JV1 = rparams.JV_Range;
                    }
                    else
                    {

                        mcounty = rparams.County;  // pCalc.county;
                        maction = rparams.Fin_action;  // pCalc.action;
                        if (rparams.Issued_Date != "Select Issue Date")
                        { issdt = rparams.Issued_Date; }

                    }



                }
                catch (Exception e)
                {
                    ErrorHandler ex = new ErrorHandler();

                    ex.ErrorLog(e);


                }
            }
            else
            {
                var pCalc = db.percentCalcs.Find(ID);
                if (pCalc == null)
                {
                    return HttpNotFound();
                }

                try
                {
                    var RVals = db.ReportParams.SingleOrDefault(x => x.nuser == username);

                    RVals.qwparam2 = pCalc.PricingGroup;
                    RVals.JV_Range = pCalc.pchoice;

                    db.SaveChanges();

                }
                catch (DbEntityValidationException e)
                {
                    ErrorHandler ex = new ErrorHandler();
                    foreach (var item in e.EntityValidationErrors)
                    {
                        ex.ErrorLog(e);
                    }

                }

                var rparams = (from rp in db.ReportParams
                               where rp.nuser == username
                               select rp).FirstOrDefault();
                //string qwhere1="";
                //    if(rparams.qwparam2 !=null){qwhere1= rparams.gparam1;}
                 mcounty = rparams.County;  // pCalc.county;
                 maction = rparams.Fin_action;  // pCalc.action;
                 W1 = rparams.gparam1;  // qwhere1; // pCalc.PricingGroup;
                 W2 = rparams.gparam2;  // pCalc.pchoice;
                 if (rparams.Issued_Date != "Select Issue Date")
                { issdt = rparams.Issued_Date; }
                 V1 = rparams.qwparam1;
                 V2 = rparams.qwparam2;
                 JV1 = rparams.JV_Range;

            }
                   


            lock (Lock)
            {
                try
                {
                    DownLoad csvexport = new DownLoad();
                    string checkStat = csvexport.csv(mcounty, maction, issdt, W1, W2, V1, V2, JV1);
                    
                    if (checkStat == "Sucesss")
                    { }
                   
                }
                catch (Exception e)
                {
                    ErrorHandler ex = new ErrorHandler();

                    ex.ErrorLog(e.InnerException);


                }
            }

                    return new EmptyResult();
            
        }

        //Market Values

        public ActionResult genmktvals(int ID) 
        {
             string username = HttpContext.User.Identity.Name.ToString();
            var pCalc = db.percentCalcs.Find(ID);
            if (pCalc == null)
            {
                return HttpNotFound();
            }

            try
            {
                var RVals = db.ReportParams.SingleOrDefault(x => x.nuser == username);
                //ReportParam RVals = new ReportParam();
                RVals.qwparam1 = pCalc.PricingGroup;
                RVals.qwparam2 = pCalc.pchoice;
                db.SaveChanges();
                
            }
            catch (DbEntityValidationException e)
            {
                ErrorHandler ex = new ErrorHandler();
                foreach (var item in e.EntityValidationErrors)
                {
                    ex.ErrorLog(e);
                }
                
            }
               
            //Tuesday
           
            var rparams = (from rp in db.ReportParams
                           where rp.nuser == username
                           select rp).FirstOrDefault();

            


            string mcounty = pCalc.county;
            string maction = pCalc.action;
            if (maction == "All")
            {
                maction = "Buy','No Buy";
            }
            string p3 = pCalc.PricingGroup;
            if (p3 == null) { p3 = " IS NULL"; } else { p3 = " = '" + p3 + "'"; }
            string p4 = pCalc.pchoice;
            if (p4 == null) { p4 = " IS NULL"; } else { p4 = " = '" + p4 + "'"; }
            string pwhere = "county='" + mcounty + "' and Fin_action in('" + maction + "') and " +  rparams.gparam1 + p3  ;
            if (rparams.gparam2 != null) { pwhere = pwhere + " and " + rparams.gparam2 +  p4; }         
            if (rparams.Issued_Date != "Select Issue Date")
            { pwhere = pwhere + " and issued_date='" + rparams.Issued_Date + "'"; }
            lock (Lock)
            {

                try
                {
                    var repgen = db.spMktVal( pwhere,  username);

                }
                catch (DataException e)
                {
                    ErrorHandler ex = new ErrorHandler();

                    ex.ErrorLog(e.InnerException);

                }
            }

            var repstrat = new reportView()
            {
                qparam1 = rparams.gparam1,
                qparam2 = rparams.gparam2,
                p1where = p3,
                p2where = p4,
                paction = maction,
                county = mcounty,
                pissdt = rparams.Issued_Date
            };

            return RedirectToAction("MarketVals");  ///, repstrat);  
        }

        //Market Values View
         public ActionResult MarketVals(string Command)  // CalcViewModel repstrat, string Command)
        {


            string cuser = HttpContext.User.Identity.Name.ToString();
            var rparams = (from rp in db.ReportParams
                           where rp.nuser == cuser
                           select rp).FirstOrDefault();
            string q1sort = rparams.gparam1;
            string q2sort = rparams.gparam2;
            string sortby = "true";
            string viewstring = "true";
            SortView chksort = new SortView();
            UpdateModel(chksort);
            if (Command == rparams.gparam1)  // repstrat.qparam1)
            {
                Command = "qparam1"; if (chksort.sFace_Value == "true")
                { viewstring = "false"; sortby = "true"; }
                else { viewstring = "true"; sortby = "false"; }
            }
            if (Command == rparams.gparam2)  // repstrat.qparam1)
            {
                Command = "qparam2"; if (chksort.sFace_Value == "true")
                { viewstring = "false"; sortby = "true"; }
                else { viewstring = "true"; sortby = "false"; }
            }
            
            if (Command == "Market Value")
            {
                if (chksort.sFace_Value == "true")
                { viewstring = "false"; sortby = "true"; }
                else { viewstring = "true"; sortby = "false"; }
            }
            if (Command == "Face Value")
            {
                if (chksort.sFace_Value == "true")
                { viewstring = "false"; sortby = "true"; }
                else { viewstring = "true"; sortby = "false"; }
            }
            if (Command == "Month 6")
            {
                if (chksort.sFace_Value == "true")
                { viewstring = "false"; sortby = "true"; }
                else { viewstring = "true"; sortby = "false"; }
            }
            if (Command == "Month 12")
            {
                if (chksort.sFace_Value == "true")
                { viewstring = "false"; sortby = "true"; }
                else { viewstring = "true"; sortby = "false"; }
            }
            if (Command == "Month 23")
            {
                if (chksort.sFace_Value == "true")
                { viewstring = "false"; sortby = "true"; }
                else { viewstring = "true"; sortby = "false"; }
            }
            if (Command == "Month 30")
            {
                if (chksort.sFace_Value == "true")
                { viewstring = "false"; sortby = "true"; }
                else { viewstring = "true"; sortby = "false"; }
            }
            if (Command == "Month 36")
            {
                if (chksort.sFace_Value == "true")
                { viewstring = "false"; sortby = "true"; }
                else { viewstring = "true"; sortby = "false"; }
            }
            if (Command == "Today")
            {
                if (chksort.sFace_Value == "true")
                { viewstring = "false"; sortby = "true"; }
                else { viewstring = "true"; sortby = "false"; }
            }



            string idate = "";
            string display_params = "";
            display_params = " -- " + rparams.gparam1 + " = " + rparams.qwparam1;
            if (rparams.gparam2 != null) { display_params = display_params + " -- " + rparams.gparam2 + " = " + rparams.qwparam2; }
            if (rparams.Issued_Date != "Select Issue Date") { idate = " -- Issue Date = " + rparams.Issued_Date; }
            ViewBag.MyTitle = rparams.County + " -- " + rparams.Fin_action + idate + display_params;         
            
            var pview = from a in db.percentCalcs orderby a.morder where a.nuser==cuser


                        select new CalcViewModel
                        {
                            ID=a.ID,
                            morder=a.morder,
                            county=a.county,
                            action = a.action,
                            PricingGroup = a.PricingGroup,
                            pchoice = a.pchoice,
                            Face_Value = a.Face_Value,
                            Month_6 = a.Month_6,
                            Month_12 = a.Month_12,
                            Month_23 = a.Month_23,
                            Month_30 = a.Month_30,
                            Month_36 = a.Month_36,
                            Today = a.Today,
                            sFace_Value = viewstring,
                            qparam1= rparams.gparam1, //  repstrat.qparam1,
                            p1where= rparams.qwparam1, // repstrat.p1where,
                            p2where=rparams.qwparam2,  // repstrat.p2where,
                            qparam2= rparams.gparam2,  //  repstrat.qparam2,
                            pissdt = rparams.Issued_Date,  // repstrat.pissdt,
                            paction=a.action
                        };

            
            switch (Command)
            {
                case "qparam1":
                    if (sortby == "true")
                    { pview = pview.OrderByDescending(s => s.PricingGroup); }
                    else { pview = pview.OrderBy(s => s.PricingGroup);  }

                    break;

                case "qparam2":
                    if (sortby == "true")
                    { pview = pview.OrderByDescending(s => s.pchoice); }
                    else { pview = pview.OrderBy(s => s.pchoice); }

                    break;

                case "Market Value":
                    if (sortby == "true")
                    { pview = pview.OrderByDescending(s => s.morder); }
                    else { pview = pview.OrderBy(s => s.morder); }

                    break;
                case "Face Value":
                    if (sortby == "true")
                    { pview = pview.OrderByDescending(s => s.Face_Value); }
                    else { pview = pview.OrderBy(s => s.Face_Value); }

                    break;


                case "Month 6":
                    if (sortby == "true")
                    { pview = pview.OrderByDescending(s => s.Month_6); }
                    else { pview = pview.OrderBy(s => s.Month_6); }

                    break;

                case "Month 12":
                    if (sortby == "true")
                    { pview = pview.OrderByDescending(s => s.Month_12); }
                    else { pview = pview.OrderBy(s => s.Month_12); }

                    break;

                case "Month 23":
                    if (sortby == "true")
                    { pview = pview.OrderByDescending(s => s.Month_23); }
                    else { pview = pview.OrderBy(s => s.Month_23); }

                    break;

                case "Month 30":
                    if (sortby == "true")
                    { pview = pview.OrderByDescending(s => s.Month_30); }
                    else { pview = pview.OrderBy(s => s.Month_30); }

                    break;

                case "Month 36":
                    if (sortby == "true")
                    { pview = pview.OrderByDescending(s => s.Month_36); }
                    else { pview = pview.OrderBy(s => s.Month_36); }

                    break;

                case "Today":
                    if (sortby == "true")
                    { pview = pview.OrderByDescending(s => s.Today); }
                    else { pview = pview.OrderBy(s => s.Today); }

                    break;

                default:

                    pview = pview.OrderBy(s => s.morder);
                    break;
            }
           
             return View(pview.ToList());

        }
    
        //Drill Down Report Values
         public ActionResult DrillDown(int ID) 
        {
            string cuser = HttpContext.User.Identity.Name.ToString();
            var pCalc = db.percentCalcs.Find(ID);
            if (pCalc == null)
            {
                return HttpNotFound();
            }

            try
            {
                var RVals = db.ReportParams.SingleOrDefault(x => x.nuser == cuser);
                //ReportParam RVals = new ReportParam();
                if (RVals != null)
                {
                    RVals.JV_Range = pCalc.pchoice;
                    //db.ReportParams.Add(RVals);
                    db.SaveChanges();
                }

            }
            catch (DbEntityValidationException e)
            {
                ErrorHandler ex = new ErrorHandler();
                foreach (var item in e.EntityValidationErrors)
                {
                    ex.ErrorLog(e);
                }

            }
           try{
              //Columns
                    var colnames = db.ColNames.Select(c => c.DDItems).Distinct();
                    List<SelectListItem> qitems = new List<SelectListItem>();
                  foreach (var t in colnames)
                  {
                      SelectListItem s = new SelectListItem();
                      s.Text = t.ToString().Trim();
                      s.Value = t.ToString().Trim();
                      qitems.Add(s);

                  }
                 
                  var rparams = (from rp in db.ReportParams
                                 where rp.nuser == cuser
                                 select rp).FirstOrDefault();
                  string idate = "";
                  string display_params = "";
                  display_params = " -- " + rparams.gparam1 + " = " + rparams.qwparam1;
                  if (rparams.gparam2 != null) { display_params = display_params + " -- " + rparams.gparam2 + " = " + rparams.qwparam2; }
                  if (rparams.Issued_Date != "Select Issue Date") { idate = " -- Issue Date = " + rparams.Issued_Date; }
                  ViewBag.MyTitle = rparams.County + " -- " + rparams.Fin_action + idate + display_params + " -- JV Range = " + rparams.JV_Range;         
            var dropitems = new selectDropDown
            {
               
                qparam1DD=qitems,
                qparam2DD=qitems
               

            };
            return View(dropitems);
           }
           catch (Exception e)
           {
               ErrorHandler ex = new ErrorHandler();
               ex.ErrorLog(e);

           }

            return View();

        }

         [HttpPost] //Drilldown Search by reportparmas
         public ActionResult DrillDown([Bind(Include = "ddparam1,ddparam2")] DrillDownView repvals)
         {
             string username =  HttpContext.User.Identity.Name.ToString();
             if (ModelState.IsValid)
             {
             try
             {
                 var RVals = db.ReportParams.SingleOrDefault(x => x.nuser == username);
                 
                 if (RVals != null)
                 {
                     RVals.gddparam1 = repvals.ddparam1;
                     RVals.gddparam2 = repvals.ddparam2;
                     //db.ReportParams.Add(RVals);
                     db.SaveChanges();
                 }

             }
             catch (DbEntityValidationException e)
             {
                 ErrorHandler ex = new ErrorHandler();
                 foreach (var item in e.EntityValidationErrors)
                 {
                     ex.ErrorLog(e);
                 }
                 //redirect to error page
             }


             //get values
             //try
             //{
                 var rparams = (from rp in db.ReportParams
                                where rp.nuser == username
                                select rp).FirstOrDefault();
                 ViewBag.MyTitle = rparams.County + " -- " + rparams.Fin_action;

                 //if (ModelState.IsValid)
                 //{
                 string lookup1 = rparams.gddparam1; // repvals.ddparam1;

                 string lookup2 = rparams.gddparam2; // repvals.ddparam2;
                 string jrange = rparams.JV_Range;  // repvals.jvalue;
                 string mcounty = rparams.County; // repvals.county;
                 string maction = rparams.Fin_action; // repvals.paction;
                 string p1 = rparams.gparam1; // repvals.col1;  // "Tax Yr";
                 string p2 = rparams.gparam2;  // repvals.col2;
                 string p3 = rparams.qwparam1; // repvals.p1where;
                 string p4 = rparams.qwparam2; // repvals.p2where;
                 string p5 = null;
                 if (rparams.Issued_Date != "Select Issue Date")
                 { p5 = rparams.Issued_Date; }
             //}
             //catch { }

                 lock (Lock)
                 {
                     //select @pgroups =' [Fin_action]=''' + @buy + ''' and CE.County = ''' + @countyname +   ''' and JV_Range =''' + @PVRange + ''' and ' + @p1group +'=''' +@p1where + '''and ' + @p2group +'=''' +@p2where + '''' + @pval2
                     //County='" + mcounty
                     
                     try
                     {
                         var repgen = db.spDrillDown(mcounty, maction, jrange, p1, p2, p3, p4, lookup1, lookup2, p5, username);
                     }
                     catch (DataException e)
                     {
                         ErrorHandler ex = new ErrorHandler();

                         ex.ErrorLog(e.InnerException);


                     }
                 }
                 return RedirectToAction("DDReport", repvals);
             }
             else
             {

                 //catch  model state
                 foreach (ModelState modelState in ViewData.ModelState.Values)
                 {
                     foreach (ModelError error in modelState.Errors)
                     {
                         string err = error.Exception.ToString();
                     }
                 }
                 //Columns
                 var colnames = db.ColNames.Select(c => c.DDItems).Distinct();
                 List<SelectListItem> qitems = new List<SelectListItem>();
                 foreach (var t in colnames)
                 {
                     SelectListItem s = new SelectListItem();
                     s.Text = t.ToString().Trim();
                     s.Value = t.ToString().Trim();
                     qitems.Add(s);

                 }
                 var dropitems = new selectDropDown
                 {
                     //jvalue = repvals.jvalue,
                     //paction = repvals.paction,
                     //county = repvals.county,
                     //col1 = repvals.col1,
                     //col2 = repvals.col2,
                     //p1where = repvals.p1where,
                     //p2where = repvals.p2where,
                     //Cert = qitems,
                     //Account = qitems,
                     //pissdt = repvals.pissdt
                     qparam1DD = qitems,
                     qparam2DD = qitems

                 };


                 return View(dropitems);
             }



         }

        //DD Report

         public ActionResult DDReport(string Command)  //[Bind(Include = "county,action,paction,pcounty,p1where,p2where,col1,col2,pricegrp,qparam1,qparam2,pissdt")] reportView repvals, string Command)
         {

             string cuser = HttpContext.User.Identity.Name.ToString();

             //Check if empty and return error page
             bool chkview = db.percentCalcs.Any(n => n.nuser.Equals(cuser));
             if (!chkview)
             {
                 ErrorHandler ex = new ErrorHandler();
                 ex.ErrrorText("No Results for: " + cuser + " Tabel is empty!");
                 return View("Index");
             }
             //get values
             var rparams = (from rp in db.ReportParams
                            where rp.nuser == cuser
                            select rp).FirstOrDefault();
             string idate = "";
             string display_params = "";
             display_params = " -- " + rparams.gparam1 + " = " + rparams.qwparam1;
             if (rparams.gparam2 != null) { display_params = display_params + " -- " + rparams.gparam2 + " = " + rparams.qwparam2; }
             if (rparams.Issued_Date != "Select Issue Date") { idate = " -- Issue Date = " + rparams.Issued_Date; }
             ViewBag.MyTitle = rparams.County + " -- " + rparams.Fin_action + idate + display_params + " -- JV Range = " + rparams.JV_Range;        
             string sortby = "true";
             string viewstring = "true";
             SortView chksort = new SortView();
             UpdateModel(chksort);
             if (Command == rparams.gddparam1)  // repvals.qparam1)
             {
                 Command = "qparam1"; if (chksort.sFace_Value == "true")
                 { viewstring = "false"; sortby = "true"; }
                 else { viewstring = "true"; sortby = "false"; }
             }
             if (Command == rparams.gddparam2)  // repvals.qparam2)
             {
                 Command = "qparam2"; if (chksort.sFace_Value == "true")
                 { viewstring = "false"; sortby = "true"; }
                 else { viewstring = "true"; sortby = "false"; }
             }

             if (Command == "Face Value")
             {
                 if (chksort.sFace_Value == "true")
                 { viewstring = "false"; sortby = "true"; }
                 else { viewstring = "true"; sortby = "false"; }
             }
             if (Command == "Month 6")
             {
                 if (chksort.sFace_Value == "true")
                 { viewstring = "false"; sortby = "true"; }
                 else { viewstring = "true"; sortby = "false"; }
             }
             if (Command == "Month 12")
             {
                 if (chksort.sFace_Value == "true")
                 { viewstring = "false"; sortby = "true"; }
                 else { viewstring = "true"; sortby = "false"; }
             }
             if (Command == "Month 23")
             {
                 if (chksort.sFace_Value == "true")
                 { viewstring = "false"; sortby = "true"; }
                 else { viewstring = "true"; sortby = "false"; }
             }
             if (Command == "Month 30")
             {
                 if (chksort.sFace_Value == "true")
                 { viewstring = "false"; sortby = "true"; }
                 else { viewstring = "true"; sortby = "false"; }
             }
             if (Command == "Month 36")
             {
                 if (chksort.sFace_Value == "true")
                 { viewstring = "false"; sortby = "true"; }
                 else { viewstring = "true"; sortby = "false"; }
             }
             if (Command == "Today")
             {
                 if (chksort.sFace_Value == "true")
                 { viewstring = "false"; sortby = "true"; }
                 else { viewstring = "true"; sortby = "false"; }
             }


             // string cuser=HttpContext.User.Identity.Name.ToString();

             //Check if empty and return error page
             var pview = from a in db.percentCalcs
                         where a.nuser == cuser

                         select new CalcViewModel()
                         {
                             ID = a.ID,
                             action = a.action,
                             PricingGroup = a.PricingGroup,
                             pchoice = a.pchoice,
                             pchoice2 = a.pchoice2,
                             Face_Value = a.Face_Value,
                             Month_6 = a.Month_6,
                             Month_12 = a.Month_12,
                             Month_23 = a.Month_23,
                             Month_30 = a.Month_30,
                             Month_36 = a.Month_36,
                             Today = a.Today,
                             qparam2 = rparams.gparam2, // repvals.qparam2,
                             pissdt = rparams.Issued_Date, // repvals.pissdt,
                             qparam1 = rparams.gparam2, // repvals.qparam1,
                             county = rparams.County, // repvals.county,
                             paction = rparams.Fin_action,  // repvals.paction,
                             ddparam1=rparams.gddparam1,
                             ddparam2=rparams.gddparam2,
                             sFace_Value = viewstring
                         };


             switch (Command)
             {
                 case "qparam1":
                     if (sortby == "true")
                     { pview = pview.OrderByDescending(s => s.PricingGroup); }
                     else { pview = pview.OrderBy(s => s.PricingGroup); }

                     break;

                 case "qparam2":
                     if (sortby == "true")
                     { pview = pview.OrderByDescending(s => s.pchoice); }
                     else { pview = pview.OrderBy(s => s.pchoice); }

                     break;
                 //case "pAccount1":
                 //    if (sortby == "true")
                 //    { pview = pview.OrderByDescending(s => s.pchoice2); }
                 //    else { pview = pview.OrderBy(s => s.pchoice2); }

                 //    break;


                 case "Face Value":
                     if (sortby == "true")
                     { pview = pview.OrderByDescending(s => s.Face_Value); }
                     else { pview = pview.OrderBy(s => s.Face_Value); }

                     break;


                 case "Month 6":
                     if (sortby == "true")
                     { pview = pview.OrderByDescending(s => s.Month_6); }
                     else { pview = pview.OrderBy(s => s.Month_6); }

                     break;

                 case "Month 12":
                     if (sortby == "true")
                     { pview = pview.OrderByDescending(s => s.Month_12); }
                     else { pview = pview.OrderBy(s => s.Month_12); }

                     break;

                 case "Month 23":
                     if (sortby == "true")
                     { pview = pview.OrderByDescending(s => s.Month_23); }
                     else { pview = pview.OrderBy(s => s.Month_23); }

                     break;

                 case "Month 30":
                     if (sortby == "true")
                     { pview = pview.OrderByDescending(s => s.Month_30); }
                     else { pview = pview.OrderBy(s => s.Month_30); }

                     break;

                 case "Month 36":
                     if (sortby == "true")
                     { pview = pview.OrderByDescending(s => s.Month_36); }
                     else { pview = pview.OrderBy(s => s.Month_36); }

                     break;

                 case "Today":
                     if (sortby == "true")
                     { pview = pview.OrderByDescending(s => s.Today); }
                     else { pview = pview.OrderBy(s => s.Today); }

                     break;

                 default:

                     pview = pview.OrderBy(s => s.PricingGroup);
                     break;
             }

             return View(pview.ToList());

         }
        //Update Issue Date 

        public ActionResult issdate(string county)
        {
            var issDt = (from c in db.Certificates where c.County == county select c.Issued_Date).Distinct();
            List<SelectListItem> ditems = new List<SelectListItem>();
            foreach (var t in issDt)
            {
                SelectListItem s = new SelectListItem();
                s.Text = Convert.ToDateTime(t).ToString("MM/dd/yyyy").Trim();
                s.Value = Convert.ToDateTime(t).ToString("MM/dd/yyyy").Trim();
                ditems.Add(s);
            }

            return Json(ditems, JsonRequestBehavior.AllowGet);

        }

        //Check Latest Redemption Date

        public ActionResult lastdate(string county)
        {

            try
            {
                var lastDt = (from c in db.Certificates where c.County == county select c.Date_Redeemed_Cert).Max();

                var dateout = Convert.ToDateTime(lastDt).ToString("MM/dd/yyyy").Trim();
                return Json(dateout, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorHandler e = new ErrorHandler();
                e.ErrorLog(ex);

            }

            return Json("No Data", JsonRequestBehavior.AllowGet);

        }

        public ActionResult test()
        {
            string maction = "No Buy";
            string mcounty = "Bay";
           // string param2 = "Cert_Status";
            var mtxyr = "2011";
           // var param1 = "Tax_yr";
            //Expression column = Expression.Parameter(typeof(TestView), tyr);
            ////IQueryable iq = w
            //var param = Expression.Parameter(typeof(TestView), "p");
            //var exp = Expression.Lambda<Func<TestView, bool>>(
            //    Expression.Equal(
            //        Expression.Property(param, tyr),
            //        Expression.Convert(Expression.Constant(mtxyr), Expression.Property(param, tyr).Type)
            //    ),
            //    param
            //);

            var fdbl = Double.Parse(mtxyr);
             var sel1 = "p.Cert_Status";
                            var sel2 = "p.Tax_yr";
                            //var gsel1 = "g.Cert_Status";
                            //var gsel2 = "g.Tax_yr";
            
            var mgroup = db.Certificates.Where("County==@0 and Fin_action==@1", new string[2] { mcounty, maction });

                            mgroup = mgroup.Where("Tax_yr ==@0",fdbl);

            var toby = mgroup.Select(x=> new
                            {
                            
                            
                            });

                           

          


            //                var dibs = mgroup.Select("new (County, Fin_action, Face_Amount,Date_Redeemed_Cert,ParcelNumber," + param1 + "," + param2 + ")");
            //               //var kdibs = dibs.Select(x => new  { });
            //var trips=dibs.Select(x=> new
            //                {
                            
                            
            //                });


            //                CalcViewModel RVals = new CalcViewModel();
            //                RVals.qaction = mgroup.
            //RVals.Fin_action = repvals.paction;
            //                RVals.Issued_Date = repvals.pissdt;
            //                RVals.County = repvals.county;
            //                RVals.gparam1 = repvals.qparam1;
            //                RVals.gparam2 = repvals.qparam2;
            //                RVals.nuser = username;
            //                db.ReportParams.Add(RVals);
            //                db.SaveChanges();
                            
                            


                           

            var query = (from g in
                            (from p in  mgroup //db.Certificates
                             join d in db.DORDatas on new { p.ParcelNumber, p.County } equals new { d.ParcelNumber, d.County } into lj
                             from lj1 in lj.DefaultIfEmpty()
                             //let myrange = (p.Date_Redeemed_Cert == null ? "Open" : "")// p.Date_Redeemed_Cert != null ? 
                             let myage = (p.Date_Redeemed_Cert != null) ? ((p.Date_Redeemed_Cert.Value.Year - p.Issued_Date.Value.Year) * 12) + ((p.Date_Redeemed_Cert.Value.Month - p.Issued_Date.Value.Month)) : 0
                             let m6 = (p.Date_Redeemed_Cert != null) && myage < 7 ? p.Face_Amount : 0
                             let m12 = (p.Date_Redeemed_Cert != null) && myage < 13 ? p.Face_Amount : 0
                             let m23 = (p.Date_Redeemed_Cert != null) && myage < 24 ? p.Face_Amount : 0
                             let m30 = (p.Date_Redeemed_Cert != null) && myage < 31 ? p.Face_Amount : 0
                             let m36 = (p.Date_Redeemed_Cert != null) && myage < 37 ? p.Face_Amount : 0
                             let mtoday= (p.Date_Redeemed_Cert != null) ? p.Face_Amount:0 
                             let pcoice1=sel1
                             let pchoice2= sel2
        //read these from params table
           // r squery = query.Select("new (County, Fin_action, Face_Amount, JV_Range, JV_Order, m6, m12, m23, m30, m36,mtoday,sel1,sel2)")
                             //select g);                                                      
                            select new { p.County, p.Fin_action, p.Face_Amount,p.Tax_yr, lj1.JV_Range, lj1.JV_Order, m6, m12, m23, m30, m36,mtoday, })
                            
            //var squery = from g in query.Where ( tyr + "="  + mtxyr + ")").AsQueryable()
                             group g by new { g.County, g.Fin_action, g.JV_Range, g.JV_Order } into f


                         select new TestView
                         {
                             action = f.Key.Fin_action,
                             county = f.Key.County,
                             pchoice = f.Key.JV_Range,
                             morder = f.Key.JV_Order,
                             Face_Amount = f.Sum(x => x.Face_Amount),
                             Month_6 = f.Sum(x => x.m6) / f.Sum(x => x.Face_Amount),
                             Month_12 = f.Sum(x => x.m12) / f.Sum(x => x.Face_Amount),
                             Month_23 = f.Sum(x => x.m23) / f.Sum(x => x.Face_Amount),
                             Month_30 = f.Sum(x => x.m30) / f.Sum(x => x.Face_Amount),
                             Month_36 = f.Sum(x => x.m36) / f.Sum(x => x.Face_Amount),
                             Today = f.Sum(x => x.mtoday) / f.Sum(x => x.Face_Amount),
                             Tax_yr=f.Max(x=>  (string)x.GetType().GetProperty("sel1").GetValue(x,null))
                             //Cert_Status = f.Max(x => x.sel2)                        
                             
                             

                         });

            //var squery=  from f in db.certDatas
            //             join q in  query

             //var squery = query.Where(tyr);


            query = query.OrderBy(s => s.morder);
            //Works!
            //var query = from g in
            //                (from p in db.Certificates
            //                 join d in db.DORDatas on new { p.ParcelNumber, p.County } equals new { d.ParcelNumber, d.County } into lj
            //                 from lj1 in lj.DefaultIfEmpty()
            //                 //let myrange = (p.Date_Redeemed_Cert == null ? "Open" : "")// p.Date_Redeemed_Cert != null ? 
            //                 let myage = (p.Date_Redeemed_Cert != null) ? ((p.Date_Redeemed_Cert.Value.Year - p.Issued_Date.Value.Year) * 12) + ((p.Date_Redeemed_Cert.Value.Month - p.Issued_Date.Value.Month)) : 0
            //                 let m6 = (p.Date_Redeemed_Cert != null) && myage < 7 ? p.Face_Amount : 0
            //                 let m12 = (p.Date_Redeemed_Cert != null) && myage < 13 ? p.Face_Amount : 0
            //                 let m23 = (p.Date_Redeemed_Cert != null) && myage < 24 ? p.Face_Amount : 0
            //                 let m30 = (p.Date_Redeemed_Cert != null) && myage < 31 ? p.Face_Amount : 0
            //                 let m36 = (p.Date_Redeemed_Cert != null) && myage < 37 ? p.Face_Amount : 0
            //                 let mtoday = (p.Date_Redeemed_Cert != null) ? p.Face_Amount : 0
            //                 //let myFace= p.Face_Amount

            //                 //query = query.Where(p.County.Contains(mcounty));
            //                 where (p.County.Contains(mcounty) && p.Fin_action.Contains(maction) && p.Tax_yr == mtxyr)

            //                 select new { p.County, p.Fin_action, p.Face_Amount, lj1.JV_Range, lj1.JV_Order, m6, m12, m23, m30, m36, mtoday }).AsQueryable()

            //            group g by new { g.County, g.Fin_action, g.JV_Range, g.JV_Order } into f



            //            select new TestView
            //            {
            //                action = f.Key.Fin_action,
            //                county = f.Key.County,
            //                pchoice = f.Key.JV_Range,
            //                morder = f.Key.JV_Order,
            //                Face_Amount = f.Sum(x => x.Face_Amount),
            //                Month_6 = f.Sum(x => x.m6) / f.Sum(x => x.Face_Amount),
            //                Month_12 = f.Sum(x => x.m12) / f.Sum(x => x.Face_Amount),
            //                Month_23 = f.Sum(x => x.m23) / f.Sum(x => x.Face_Amount),
            //                Month_30 = f.Sum(x => x.m30) / f.Sum(x => x.Face_Amount),
            //                Month_36 = f.Sum(x => x.m36) / f.Sum(x => x.Face_Amount),
            //                Today = f.Sum(x => x.mtoday) / f.Sum(x => x.Face_Amount)


            //            };

           


//            query = query.OrderBy(s => s.morder);

           


            return View(query);
        }



    }
}