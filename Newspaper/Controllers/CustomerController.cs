using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newspaper.Models;
//using PagedList;
using Newspaper.Models.DAL;
using System.Net.Mail;
using System.Data.SqlClient;
using Newspaper.Models.ViewModel;
using Newspaper.Filters;
using System.Configuration;

using System.IO;

namespace Newspaper.Controllers
{
    [SessionCheck(Role = "Supervisor,Admin,SuperAdmin")]
    [ValidateInput(false)]
    public class CustomerController : Controller
    {
        private activities log = new activities();
        private NewspaperEntities db = new NewspaperEntities();

        // GET: /Customer/
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, Customer customer)
        {
            if (Session["Category"].ToString() == "SuperAdmin" || Session["Category"].ToString() == "Counter")
            {
                var Cus = db.Customer.ToList();
                return View(Cus.ToList());

            }

            var Customer = db.Customer.ToList();
            return View(Customer.ToList());

            //int BranchId = Convert.ToInt32(Session["BranchId"].ToString());
            //var customers = db.Customer.Where(m=>m. BranchId==BranchId).ToList();
            //            var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
            //  .Select(x => new { x.Key, x.Value.Errors }).ToArray();
            //return View(customers.ToList());
        }

        public ActionResult AssignedCustomer()
        {
            var assigned = db.ServiceAssign.ToList();
            return View(assigned.ToList());

        }


        // GET: /Customer/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customer.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: /Customer/Create
        public ActionResult Create()
        {
            ViewBag.SalesManId = new SelectList(db.SalesMan, "Id", "FullName");
            ViewBag.ServiceId = new SelectList(db.Service, "Id", "NewsPaperName");
            ViewBag.BranchId = new SelectList(db.Branch, "BranchId", "BranchName");
            ViewBag.OfficerId = new SelectList(db.Officers.Where(m => m.Status && m.OfficerType == "कम्प्लिमेन्ट"), "Id", "Name");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.SalesManId = new SelectList(db.SalesMan, "Id", "FullName");
                ViewBag.ServiceId = new SelectList(db.Service, "Id", "NewsPaperName");
                ViewBag.BranchId = new SelectList(db.Branch, "BranchId", "BranchName");
                ViewBag.OfficerId = new SelectList(db.Officers.Where(m => m.Status && m.OfficerType == "कम्प्लिमेन्ट"), "Id", "Name");

                return View(customer);
            }
            else
            {
                customer.RegisterDate = DateTime.Now;
                customer.RegisteredBy = Session["userEmail"].ToString();
                db.Customer.Add(customer);
                db.SaveChanges();

                try
                {

                    log.AddActivity("Customer Created Successfully");

                    return RedirectToAction("index");
                }
                catch
                {
                    return View();
                }
            }
        }


        // GET: /Customer/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customer.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            ViewBag.BranchId = new SelectList(db.Branch, "BranchId", "BranchName", customer.BranchId);
            ViewBag.OfficerId = new SelectList(db.Officers.Where(m => m.Status && m.OfficerType == "कम्प्लिमेन्ट"), "Id", "Name",customer.OfficerId!=null?customer.OfficerId:null);
            return View(customer);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Customer customer)
        {
            ViewBag.OfficerId = new SelectList(db.Officers.Where(m => m.Status && m.OfficerType == "कम्प्लिमेन्ट"), "Id", "Name", customer.OfficerId != null ? customer.OfficerId : null);

            if (ModelState.IsValid)
            {
                customer.RegisterDate = DateTime.Now;
                customer.RegisteredBy = Session["userEmail"].ToString();
                db.Entry(customer).State = EntityState.Modified;
                try
                {

                    db.SaveChanges();

                    String Operation = "Customer Updated Sucessfully";
                    db.ActivityLog.Add(new ActivityLog
                    {
                        BranchId = customer.BranchId,
                        Operation = Operation,
                        CreatedBy = Session["userEmail"].ToString(),
                        CreatedDate = DateTime.Now

                    });
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch
                {
                    return View();
                }
            }
            return View(customer);

        }


        [SessionCheck(Role = "SuperAdmin")]
        //Delete Customer and Service assigned to the customer
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customer.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            else
            {
                string newspapers = "no services";

                IEnumerable<ServiceAssign> services = db.ServiceAssign.Where(m => m.CustomerId == id);

                if (services != null)
                {
                    newspapers = String.Join(",", services.Select(m => m.Newspapers.Select(n => n.NewspaperName)));
                }
                //Remove all the sevices assigned to the customer
                foreach (ServiceAssign service in services)
                {
                    db.ServiceAssign.Remove(service);
                    db.SaveChanges();
                }

                string activeLog = customer.FirstName + " Deleted by SuperAdmin with services " + services;

                //Remove Customers
                db.Customer.Remove(customer);
                db.SaveChanges();

                //Update Activity log 
                db.ActivityLog.Add(new ActivityLog
                {
                    BranchId = null,
                    Operation = activeLog,
                    CreatedBy = Session["userEmail"].ToString(),
                    CreatedDate = DateTime.Now
                });
                db.SaveChanges();
            }
            return View(customer);
        }

        // POST: /Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = db.Customer.Find(id);
            db.Customer.Remove(customer);
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


        [HttpPost]
        public JsonResult getEmployee(int id)
        {
            Customer objemp = db.Customer.FirstOrDefault(m => m.Id == id);
            return Json(objemp);
        }



    }
}

