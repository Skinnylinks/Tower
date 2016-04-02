using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Tower.Models;
using System.Web.Routing;

namespace Tower.Controllers
{
    [Authorize(Roles = "Admin")]
    public class contactsController : Controller
    {
        private TowerEntities db = new TowerEntities();

        // GET: contacts
        public ActionResult Index(int? id)
        {


            var contact = (from c in db.contacts where c.CID == id select c).ToList();

            if (contact.Count == 0)
            {
                List<contact> mylist = new List<Models.contact>();

                var toby = new contact
                {
                    CID = (int)id,
                    adddate = DateTime.Now
                };
                mylist.Add(toby);
                return View(mylist.AsEnumerable());
            }

            return View(contact);
            //return View(db.contacts.ToList());
        }

        // GET: contacts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            contact contact = db.contacts.Find(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }

        // GET: contacts/Create
        public ActionResult Create(int? lid)
        {
            var cmodel = new contact
            {

                CID = (int)lid,
                adddate = DateTime.Now
            };

            return View(cmodel);
        }

        // POST: contacts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CNTYID,CID,ContactFirstName,ContactLastName,faxnumber,PhoneNumber,EmailAddress,adddate,notes,ext,cellphone")] contact contact)
        {
            if (ModelState.IsValid)
            {
                db.contacts.Add(contact);
                db.SaveChanges();

                return RedirectToAction("Details", new RouteValueDictionary(
    new { controller = "county", action = "Details", Id = contact.CID }));

                // return RedirectToAction("Index");
            }

            return View(contact);
        }

        // GET: contacts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            contact contact = db.contacts.Find(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }

        // POST: contacts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CNTYID,CID,ContactFirstName,ContactLastName,faxnumber,PhoneNumber,EmailAddress,adddate,notes,ext,cellphone")] contact contact)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contact).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new RouteValueDictionary(
   new { controller = "county", action = "Details", Id = contact.CID }));

                //return RedirectToAction("Index");
            }
            return View(contact);
        }

        // GET: contacts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            contact contact = db.contacts.Find(id);
            if (contact == null)
            {
                return View(contact);
                //return HttpNotFound();
            }
            return View(contact);
        }

        // POST: contacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            contact contact = db.contacts.Find(id);
            db.contacts.Remove(contact);
            db.SaveChanges();
            return RedirectToAction("Details", new RouteValueDictionary(
   new { controller = "county", action = "Details", Id = contact.CID }));
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