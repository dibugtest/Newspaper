using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Newspaper.Models;
using Newspaper.Models.DAL;
using Newspaper.Filters;

namespace Newspaper.Controllers
{
    [SessionCheck(Role = "SuperAdmin")]
    [ValidateInput(false)]
    public class OfficerController : Controller
    {
        private activities log = new activities();
        private NewspaperEntities db = new NewspaperEntities();

        // GET: /Officer/
        public ActionResult Index()
        {
            if (Session["Category"].ToString() == "SuperAdmin")
            {
                var Officers = db.Officers.ToList();
                return View(Officers);
            }
            else
            {
                return View();
            }

        }

        // GET: /Officer/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Officer officer = db.Officers.Find(id);
            if (officer == null)
            {
                return HttpNotFound();
            }
            return View(officer);
        }

        // GET: /Officer/Create
        public ActionResult Create()
        {
            ViewBag.OfficerType = new List<SelectListItem> { new SelectListItem{Text = "---छान्नुहोस---", Value = ""},
                                                            new SelectListItem{Text="कम्प्लिमेन्ट",Value="कम्प्लिमेन्ट" },
                                                            new SelectListItem{Text="नर्मल",Value="नर्मल" } };
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Officer officer)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.OfficerType = new List<SelectListItem> { new SelectListItem{Text = "---छान्नुहोस---", Value = ""},
                                                            new SelectListItem{Text="कम्प्लिमेन्ट",Value="कम्प्लिमेन्ट" },
                                                            new SelectListItem{Text="नर्मल",Value="नर्मल" } };
                return View(officer);
            }
            else
            {
                officer.CreatedBy = Session["userEmail"].ToString();
                officer.CreatedDate = DateTime.Now;
                officer.UpdatedDate = DateTime.Now;
                db.Officers.Add(officer);
                db.SaveChanges();

                try
                {
                    log.AddActivity("Officer Created Successfully");

                    return RedirectToAction("index");
                }
                catch
                {
                    return View();
                }
            }
        }


        // GET: /Officer/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Officer officer = db.Officers.Find(id);
            if (officer == null)
            {
                return HttpNotFound();
            }
            List<SelectListItem> officerTypes = new List<SelectListItem> { new SelectListItem{Text = "---छान्नुहोस---", Value = ""},
                                                            new SelectListItem{Text="कम्प्लिमेन्ट",Value="कम्प्लिमेन्ट" },
                                                            new SelectListItem{Text="नर्मल",Value="नर्मल" } };
            ViewBag.OfficerType = new SelectList(officerTypes, "Value", "Text",officer.OfficerType);
            return View(officer);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Officer officer)
        {
            List<SelectListItem> officerTypes = new List<SelectListItem> { new SelectListItem{Text = "---छान्नुहोस---", Value = ""},
                                                            new SelectListItem{Text="कम्प्लिमेन्ट",Value="कम्प्लिमेन्ट" },
                                                            new SelectListItem{Text="नर्मल",Value="नर्मल" } };
            ViewBag.OfficerType = new SelectList(officerTypes, "Value", "Text", officer.OfficerType);
            if (ModelState.IsValid)
            {
                Officer off = db.Officers.Find(officer.Id);
                off.UpdatedDate=DateTime.Now;
                off.Status = officer.Status;
                off.PISNo = officer.PISNo;
                off.Name = officer.Name;
                off.OfficeAddress = officer.OfficeAddress;
                off.OfficerType = officer.OfficerType;
                off.Phone = officer.Phone;
                off.Email = officer.Email;

                try
                {
                    db.SaveChanges();

                    string Operation = "Officer Updated Sucessfully";
                    db.ActivityLog.Add(new ActivityLog
                    {
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

            return View(officer);

        }


        [SessionCheck(Role = "SuperAdmin")]
        //Delete Officer and Service assigned to the Officer
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Officer officer = db.Officers.Find(id);
            if (officer == null)
            {
                return HttpNotFound();
            }
            else
            {

                string activeLog = "Officer Deleted by SuperAdmin";

                //Remove Officers
                db.Officers.Remove(officer);
                db.SaveChanges();

                //Update Activity log 
                db.ActivityLog.Add(new ActivityLog
                {
                    Operation = activeLog,
                    CreatedBy = Session["userEmail"].ToString(),
                    CreatedDate = DateTime.Now
                });
                db.SaveChanges();
            }
            return View(officer);
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

