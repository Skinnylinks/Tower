using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Web;
using System.Net;
using System.Web.Mvc;
using Tower.Models;
using PagedList;
using Tower.Utils;
using System.Data.Entity.Validation;
using Tower.ViewModels;
using System.Web.Routing;

namespace Tower.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CountyController : Controller
    {
        private TowerEntities db = new TowerEntities();

        // GET: county

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "county" : "";
            //ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var sib_customers = from s in db.customers
                                select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                sib_customers = sib_customers.Where(s => s.CountyName.Contains(searchString));
                //|| s.FirstMidName.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "county":
                    sib_customers = sib_customers.OrderByDescending(s => s.CountyName);
                    break;
                //case "Date":
                //    students = students.OrderBy(s => s.EnrollmentDate);
                //    break;
                //case "date_desc":
                //    students = students.OrderByDescending(s => s.EnrollmentDate);
                //    break;
                default:
                    sib_customers = sib_customers.OrderBy(s => s.CountyName);
                    break;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(sib_customers.ToPagedList(pageNumber, pageSize));


            //return View(db.customers.ToList());
        }

        // GET: county/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            customer customer = db.customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: county/Create

        public ActionResult Create()
        {
            return View();
        }

        // POST: county/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CID,CountyName,ContactFirstName,ContactLastName,BillingAddress,City,StateOrProvince,PostalCode,Country,faxnumber,PhoneNumber,EmailAddress,dataUrl,adddate,notes,clerkUrl,countyUrl,ext,cellphone")] customer customer)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.customers.Add(customer);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DbEntityValidationException e)
                {
                    
                    // Retrieve the error messages as a list of strings.
                    var errorMessages = e.EntityValidationErrors
                            .SelectMany(x => x.ValidationErrors)
                            .Select(x => x.ErrorMessage);

                    // Join the list to a single string.
                    var fullErrorMessage = string.Join("; ", errorMessages);

                    // Combine the original exception message with the new one.
                    var exceptionMessage = string.Concat(e.Message, " The validation errors are: ", fullErrorMessage);
                    ErrorHandler ex = new ErrorHandler();
                    ex.ErrrorText(exceptionMessage);
                    foreach (var item in e.EntityValidationErrors)
                    {
                        ex.ErrorLog(e);
                       
                    }

                    return RedirectToAction("Error", new RouteValueDictionary(
   new { controller = "county", action = "Error", exceptionMessage }));
                    //return RedirectToAction("Error" );

                }
            }

            return View(customer);
        }

        public ActionResult Error(string exceptionMessage)
        {

            List<ErrorView> mylist = new List<ViewModels.ErrorView>();

            var myError = new ErrorView
            {
                message = "You broke me!" + exceptionMessage
            };
            mylist.Add(myError);
            return View(mylist.AsEnumerable());
                   

        }

        // GET: county/Edit/5

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            customer customer = db.customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: county/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CID,CountyName,ContactFirstName,ContactLastName,BillingAddress,City,StateOrProvince,PostalCode,Country,faxnumber,PhoneNumber,EmailAddress,dataUrl,adddate,notes,clerkUrl,countyUrl,ext,cellphone")] customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        // GET: county/Delete/5

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            customer customer = db.customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: county/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            customer customer = db.customers.Find(id);
            db.customers.Remove(customer);
            db.SaveChanges();
            return RedirectToAction("Index");
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