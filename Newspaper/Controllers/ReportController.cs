using Newspaper.Filters;
using Newspaper.Models;
using Newspaper.Models.DAL;
using Newspaper.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Newspaper.Controllers
{
    [SessionCheck(Role = "SuperAdmin,Supervisor")]
    public class ReportController : Controller
    {
        private activities log = new activities();

        private NewspaperEntities db = new NewspaperEntities();
        // GET: Report
        public ActionResult Index()
        {
            var CustomerList = db.Customer.ToList();

            return View(CustomerList);
        }

        public ActionResult ExpiredReport()
        {

            var Newspaper = (from p in db.ServiceAssign
                             from c in db.Customer
                             from s in db.SalesMan
                             from e in db.Service

                             where p.CustomerId == c.Id && p.SalesManId == s.Id && p.NewspaperId == e.Id && p.EndedDate <= DateTime.Now
                             select new
                             {
                                 serviceAssign = p,
                                 customer = c,
                                 salesman = s,
                                 newspaper = e
                             }).ToList();

            List<CounterVM> listCounter = new List<CounterVM>();

            for (int i = 0; i < Newspaper.ToList().Count; i++)
            {
                CounterVM objcounter = new CounterVM();
                objcounter.ServiceAssign = Newspaper[i].serviceAssign;
                objcounter.Customer = Newspaper[i].customer;
                objcounter.NewsPaper = Newspaper[i].newspaper;


                objcounter.SalesMan = Newspaper[i].salesman;
                objcounter.enddate = ADTOBS.EngToNep(Newspaper[i].serviceAssign.EndedDate).ToString();
                listCounter.Add(objcounter);
                // AddedCustomerVM.Add(new AddedCustomerVM { NewsPaperName = Newspaper[i].NewspaperName, NepaliDate = AddedDate, SalesMan = Newspaper[i].SalesMan/*, NewsPaperName =Newspaper[i].NewspaperName*/, ReportDate = Newspaper[i].date, FirstName = Newspaper[i].FirstName, Address = Newspaper[i].Address, MPhone = Newspaper[i].MPhone, PaperDispatchDate = Newspaper[i].PaperDispatchDate, EndedDate = Newspaper[i].EndedDate, CustomerId = Newspaper[i].CustomerId.ToString(), branch = "All Branches" });

            }


            // var Count = db.ServiceAssign.Where(t => t.EndedDate <= DateTime.Now).ToList();
            if (Newspaper.Count == 0)
            {
                TempData["message"] = "No one is expired";
                return RedirectToAction("Report", "DayWiseCount");
            }
            log.AddActivity("Expired Customers reported printed successfully");
            return View(listCounter);
        }

        //start

        public ActionResult SelectRewNewspaper()
        {

            ViewBag.ServiceId = new SelectList(db.Service, "Id", "NewsPaperName");
            return View();
        }

        public ActionResult RenewCustomerReport(List<AddedCustomerVM> AddedCustomerVM)
        {
            return RedirectToAction("SelectRewNewspaper");
        }
        [HttpPost]
        public ActionResult RenewCustomerReport(FormCollection col)
        {
            List<AddedCustomerVM> AddedCustomerVM = new List<AddedCustomerVM>();
            int ServiceId = Convert.ToInt32(col["ServiceId"]);

            string AddedDate = col["NepDate"].ToString();
            DateTime date = Convert.ToDateTime(col["RegisterDate"]);

            NewspaperEntities db = new NewspaperEntities();
            {
                var Newspaper = (from p in db.ServiceAssign
                                 from c in db.Customer
                                 from s in db.SalesMan
                                 from e in db.Service
                                     // replace  && p.CustomerId == c.Id && p.SalesManId == s.Id && p.NewspaperId == e.Id 
                                     //where p.NewspaperId == e.Id && p.CreatedDate == date && p.CustomerId.ToString() == c.CustomerId && p.SalesManId == s.Id && p.NewspaperId == ServiceId
                                 where p.NewspaperId == ServiceId && p.UpdatedDate == date && p.PaymentType == true && p.CustomerId == c.Id && p.SalesManId == s.Id && p.NewspaperId == e.Id
                                 select new
                                 {
                                     serviceId = p.NewspaperId,
                                     FirstName = c.FirstName,
                                     quantity = p.Quantity,
                                     NepaliDate = p.NepaliDate,
                                     SalesMan = s.FullName,
                                     MPhone = c.MPhone,
                                     Address = c.Address,
                                     PaperDispatchDate = p.PaperDispatchDate,
                                     EndedDate = p.EndedDate,

                                     NewspaperName = e.NewsPaperName,
                                     date = p.UpdatedDate,
                                     CustomerId = c.CustomerId
                                 }).ToList();

                if (Newspaper == null || Newspaper.Count == 0)
                {
                    TempData["message"] = "No record found.";
                    return RedirectToAction("SelectRewNewspaper");
                }
                for (int i = 0; i < Newspaper.ToList().Count; i++)
                {
                    if (Newspaper[i].CustomerId == null)
                    {
                        AddedCustomerVM.Add(new AddedCustomerVM { dispatch = ADTOBS.EngToNep(Newspaper[i].PaperDispatchDate).ToString(), enddate = ADTOBS.EngToNep(Newspaper[i].EndedDate).ToString(), NewsPaperName = Newspaper[i].NewspaperName, NepaliDate = AddedDate, SalesMan = Newspaper[i].SalesMan/*, NewsPaperName =Newspaper[i].NewspaperName*/, ReportDate = Newspaper[i].date, FirstName = Newspaper[i].FirstName, quantity = Convert.ToInt32(Newspaper[i].quantity).ToString(), Address = Newspaper[i].Address, MPhone = Newspaper[i].MPhone, PaperDispatchDate = Newspaper[i].PaperDispatchDate, EndedDate = Newspaper[i].EndedDate, branch = "All Branches" });
                    }
                    else
                    {



                        AddedCustomerVM.Add(new AddedCustomerVM
                        {
                            dispatch = ADTOBS.EngToNep(Newspaper[i].PaperDispatchDate).ToString(),
                            enddate = ADTOBS.EngToNep(Newspaper[i].EndedDate).ToString(),
                            NewsPaperName = Newspaper[i].NewspaperName,
                            quantity = Convert.ToInt32(Newspaper[i].quantity).ToString(),
                            NepaliDate = AddedDate,
                            SalesMan = Newspaper[i].SalesMan/*, NewsPaperName =Newspaper[i].NewspaperName*/,
                            ReportDate = Newspaper[i].date,
                            FirstName = Newspaper[i].FirstName,
                            Address = Newspaper[i].Address,
                            MPhone = Newspaper[i].MPhone,
                            PaperDispatchDate = Newspaper[i].PaperDispatchDate,
                            EndedDate = Newspaper[i].EndedDate,
                            CustomerId = Newspaper[i].CustomerId.ToString(),
                        });

                    }
                }
                if (AddedCustomerVM == null)
                {
                    TempData["message"] = "No record found.";
                    return RedirectToAction("SelectRewNewspaper");
                }
                try
                {

                    String Operation = "Renew Customer Report Printed";
                    db.ActivityLog.Add(new ActivityLog
                    {

                        Operation = Operation,
                        CreatedBy = Session["userEmail"].ToString(),
                        CreatedDate = DateTime.Now

                    });
                    db.SaveChanges();
                }
                catch
                {
                    ViewBag.ErrorMessage = "Email sending failed";
                }
                return View(AddedCustomerVM);
            }


        }


        public ActionResult SelectNewspaper()
        {

            ViewBag.ServiceId = new SelectList(db.Service, "Id", "NewsPaperName");
            return View();
        }

        public ActionResult AddedCustomerReport(List<AddedCustomerVM> AddedCustomerVM)
        {
            return RedirectToAction("SelectNewspaper");
        }
        [HttpPost]
        public ActionResult AddedCustomerReport(FormCollection col)
        {
            List<AddedCustomerVM> AddedCustomerVM = new List<AddedCustomerVM>();
            int ServiceId = Convert.ToInt32(col["ServiceId"]);

            string AddedDate = col["NepDate"].ToString();
            DateTime date = Convert.ToDateTime(col["RegisterDate"]);

            NewspaperEntities db = new NewspaperEntities();
            {
                var Newspaper = (from p in db.ServiceAssign
                                 from c in db.Customer
                                 from s in db.SalesMan
                                 from e in db.Service
                                 where p.NewspaperId == ServiceId && p.UpdatedDate == date && p.PaymentType == false && p.CustomerId == c.Id && p.SalesManId == s.Id && p.NewspaperId == e.Id
                                 select new
                                 {
                                     serviceId = p.NewspaperId,
                                     FirstName = c.FirstName,
                                     NepaliDate = p.NepaliDate,
                                     SalesMan = s.FullName,
                                     quantity = p.Quantity,
                                     MPhone = c.MPhone,
                                     Address = c.Address,
                                     PaperDispatchDate = p.PaperDispatchDate,
                                     EndedDate = p.EndedDate,
                                     NewspaperName = e.NewsPaperName,
                                     date = c.RegisterDate,
                                     CustomerId = c.CustomerId
                                 }).ToList();

                if (Newspaper == null || Newspaper.Count == 0)
                {
                    TempData["message"] = "No record found.";
                    return RedirectToAction("SelectNewspaper");
                }
                for (int i = 0; i < Newspaper.ToList().Count; i++)
                {
                    if (Newspaper[i].CustomerId == null)
                    {

                        AddedCustomerVM.Add(new AddedCustomerVM
                        {
                            dispatch = ADTOBS.EngToNep(Newspaper[i].PaperDispatchDate).ToString(),
                            enddate = ADTOBS.EngToNep(Newspaper[i].EndedDate).ToString(),
                            NewsPaperName = Newspaper[i].NewspaperName,
                            NepaliDate = AddedDate,
                            SalesMan = Newspaper[i].SalesMan,
                            quantity = Convert.ToInt32(Newspaper[i].quantity).ToString(),
                            ReportDate = Newspaper[i].date,
                            FirstName = Newspaper[i].FirstName,
                            Address = Newspaper[i].Address,
                            MPhone = Newspaper[i].MPhone,
                            PaperDispatchDate = Newspaper[i].PaperDispatchDate,
                            EndedDate = Newspaper[i].EndedDate,

                            branch = "All Branches"
                        });
                    }
                    else
                    {
                        AddedCustomerVM.Add(new AddedCustomerVM
                        {
                            dispatch = ADTOBS.EngToNep(Newspaper[i].PaperDispatchDate).ToString(),
                            enddate = ADTOBS.EngToNep(Newspaper[i].EndedDate).ToString(),
                            NewsPaperName = Newspaper[i].NewspaperName,
                            quantity = Convert.ToInt32(Newspaper[i].quantity).ToString(),
                            NepaliDate = AddedDate,
                            SalesMan = Newspaper[i].SalesMan,
                            ReportDate = Newspaper[i].date,
                            FirstName = Newspaper[i].FirstName,
                            Address = Newspaper[i].Address,
                            MPhone = Newspaper[i].MPhone,
                            PaperDispatchDate = Newspaper[i].PaperDispatchDate,
                            EndedDate = Newspaper[i].EndedDate,
                            CustomerId = Newspaper[i].CustomerId.ToString(),
                            branch = "All Branches"
                        });
                    }
                }
                if (AddedCustomerVM == null)
                {
                    TempData["message"] = "No record found.";
                    return RedirectToAction("SelectNewspaper");
                }
                try
                {

                    String Operation = "Added Customer Report Printed";
                    db.ActivityLog.Add(new ActivityLog
                    {

                        Operation = Operation,
                        CreatedBy = Session["userEmail"].ToString(),
                        CreatedDate = DateTime.Now

                    });
                    db.SaveChanges();
                }
                catch
                {
                    ViewBag.ErrorMessage = "Email sending failed";
                }
                return View(AddedCustomerVM);
            }


        }



        public ActionResult SelectDate()
        {
            //List<Branch> listBranch = db.Branch.ToList();
            //listBranch.Add(new Branch { BranchId = 0, BranchName = "All Branches" });
            //ViewBag.BranchId = new SelectList(listBranch, "BranchId", "BranchName");
            return View();
        }
        public ActionResult ExpiredCustomerBydate(List<AddedCustomerVM> AddedCustomerVM)
        {
            return RedirectToAction("SelectDate");
        }
        [HttpPost]
        public ActionResult ExpiredCustomerBydate(FormCollection col)
        {

            List<AddedCustomerVM> AddedCustomerVM = new List<AddedCustomerVM>();
            string EndedDate = col["NepDate"].ToString();
            DateTime date = Convert.ToDateTime(col["EndedDate"]);

            NewspaperEntities db = new NewspaperEntities();

            var Newspaper = (from s in db.ServiceAssign
                             from e in db.Service
                             from d in db.SalesMan
                             from c in db.Customer
                             where s.EndedDate == date && s.CustomerId == c.Id && s.SalesManId == d.Id && s.NewspaperId == e.Id
                             select new
                             {
                                 ServiceId = s.NewspaperId,
                                 FirstName = c.FirstName,
                                 SalesMan = d.FullName,
                                 MPhone = c.MPhone,
                                 Address = c.Address,
                                 NewspaperName = e.NewsPaperName,
                                 date = s.EndedDate,
                                 CustomerId = c.CustomerId
                             }).ToList();






            if (Newspaper == null || Newspaper.Count == 0)
            {
                TempData["message"] = "No record found.";
                return RedirectToAction("SelectDate");
            }
            for (int i = 0; i < Newspaper.ToList().Count; i++)
            {

                AddedCustomerVM.Add(new AddedCustomerVM { enddate = ADTOBS.EngToNep(date).ToString(), EndedDate = date, NepaliDate = EndedDate, SalesMan = Newspaper[i].SalesMan, NewsPaperName = Newspaper[i].NewspaperName, ReportDate = Newspaper[i].date, FirstName = Newspaper[i].FirstName, Address = Newspaper[i].Address, MPhone = Newspaper[i].MPhone, CustomerId = Newspaper[i].CustomerId });

            }
            if (AddedCustomerVM == null)
            {
                TempData["message"] = "No record found.";
                return RedirectToAction("SelectDistributorDate");
            }
            try
            {

                String Operation = "Expired Reported printed";

                db.ActivityLog.Add(new ActivityLog
                {

                    Operation = Operation,
                    CreatedBy = Session["userEmail"].ToString(),
                    CreatedDate = DateTime.Now

                });
                db.SaveChanges();

            }
            catch
            {
                ViewBag.ErrorMessage = "Email sending failed";

            }

            return View(AddedCustomerVM);

        }



        public ActionResult SelectDistributorDate()
        {
            ViewBag.ServiceId = new SelectList(db.Service, "Id", "NewsPaperName");
            return View();
        }

        public ActionResult DistributorAddReport(List<DistributorVM> DistributorVM)
        {
            return RedirectToAction("SelectDistributorDate");
        }

        [HttpPost]
        public ActionResult DistributorAddReport(FormCollection col)
        {

            List<DistributorVM> DistributorVM = new List<DistributorVM>();
            int ServiceId = Convert.ToInt32(col["ServiceId"]);
            string EndedDate = col["NepDate"].ToString();
            DateTime date = Convert.ToDateTime(col["EndedDate"]);
            DateTime yes = date.AddDays(-1);
            NewspaperEntities db = new NewspaperEntities();
            int serviceId = Convert.ToInt32(col["ServiceId"].ToString());

            var objService = db.Service.Find(serviceId);

            var paperAll = db.ServiceAssign.Where(m => m.EndedDate >= date && m.PaperDispatchDate <= date).Where(m => m.NewspaperId == serviceId).GroupBy(n => n.SalesManId)
                                   .Select(g => new { FullName = g.Key, SalesManId = g.Key, Count = g.Sum(m => m.Quantity) }).ToList();

            var PaperTotal = db.ServiceAssign.Where(m => m.EndedDate >= yes && m.PaperDispatchDate <= yes).Where(m => m.NewspaperId == serviceId).GroupBy(n => n.SalesManId)
                                   .Select(g => new { FullName = g.Key, SalesManId = g.Key, Count = g.Sum(m => m.Quantity) }).ToList();
            var substracted = db.ServiceAssign.Where(m => m.EndedDate == date).Where(m => m.NewspaperId == serviceId).GroupBy(n => n.SalesManId)
                                    .Select(g => new { FullName = g.Key, SalesManId = g.Key, Count = g.Sum(m => m.Quantity) }).ToList();

            var added = db.ServiceAssign.Where(m => m.PaperDispatchDate == date).Where(m => m.NewspaperId == serviceId).GroupBy(n => n.SalesManId)
                                   .Select(g => new { FullName = g.Key, SalesManId = g.Key, Count = g.Sum(m => m.Quantity) }).ToList();

            for (int i = 0; i < paperAll.Count; i++)
            {
                int addedInt = 0, substractInt = 0, paperTotalInt = 0;
                if (PaperTotal.FirstOrDefault(m => m.SalesManId == paperAll[i].SalesManId) == null)
                {
                    paperTotalInt = 0;
                }
                else
                {
                    paperTotalInt = PaperTotal.FirstOrDefault(m => m.SalesManId == paperAll[i].SalesManId).Count;
                }

                if (added.FirstOrDefault(m => m.SalesManId == paperAll[i].SalesManId) == null)
                {
                    addedInt = 0;
                }
                else
                {
                    addedInt = added.FirstOrDefault(m => m.SalesManId == paperAll[i].SalesManId).Count;
                }

                if (substracted.FirstOrDefault(m => m.SalesManId == paperAll[i].SalesManId) == null)
                {
                    substractInt = 0;
                }
                else
                {
                    substractInt = substracted.FirstOrDefault(m => m.SalesManId == paperAll[i].SalesManId).Count;
                }


                DistributorVM.Add(new DistributorVM
                {
                    DistributorName = db.SalesMan.Find(paperAll[i].SalesManId).FullName,
                    newspaperName = objService.NewsPaperName,
                    Quantity = paperTotalInt,

                    Added = addedInt,
                    Subs = substractInt,
                    NepaliDate = EndedDate,
                });

            }
            try
            {
                string operation = "Distributor Report is created";
                db.ActivityLog.Add(new ActivityLog
                {
                    Operation = operation,
                    CreatedBy = Session["userEmail"].ToString(),
                    CreatedDate = DateTime.Now,
                });
                db.SaveChanges();
            }
            catch
            {
                ViewBag.ErrorMessage = "Distributor Report Failed to print";
            }
            if (DistributorVM.Count == 0)
            {
                TempData["message"] = "No record found.";
                return RedirectToAction("SelectDistributorDate");
            }

            return View(DistributorVM);

        }


        public ActionResult ActiveReport()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ActiveReport(SummaryReport rep)
        {
            if (ModelState.IsValid)
            {
                //Start of Report Items
                IEnumerable<int> serviceIds = db.Service.Select(m => m.Id);

                var actives = (from s in db.ServiceAssign
                               from c in db.Customer
                               from p in db.Service
                               where s.CustomerId == c.Id && s.NewspaperId == p.Id
                               && s.EndedDate >= rep.EndedDate// && s.PaperDispatchDate>=rep.StartDate  && s.PaperDispatchDate<=rep.EndedDate
                               select new
                               {
                                   customer = c,
                                   ServiceAssign = s,
                                   service = p
                               }).GroupBy(m => m.service.Id).Select(m => new
                               {
                                   serviceId = m.Key,
                                   count = m.Count(),
                                   customerIds = db.ServiceAssign.Where(x => x.NewspaperId == m.Key).Select(x => x.CustomerId)

                               }).ToList();

                actives = actives.Where(m => serviceIds.Contains(m.serviceId)).ToList();

                var activeCustomer = actives.Select(m => new
                {
                    service = db.Service.Find(m.serviceId),
                    customers = db.Customer.Where(x => m.customerIds.Contains(x.Id)),
                    count = m.count

                });

                var totals = (from s in db.ServiceAssign
                              from c in db.Customer
                              from p in db.Service
                              where s.CustomerId == c.Id && s.NewspaperId == p.Id
                              select new
                              {
                                  customer = c,
                                  ServiceAssign = s,
                                  service = p
                              }).GroupBy(m => m.service.Id).Select(m => new
                              {
                                  serviceId = m.Key,
                                  count = m.Count(),
                                  customerIds = db.ServiceAssign.Where(x => x.NewspaperId == m.Key).Select(x => x.CustomerId)
                              }).ToList();

                totals = totals.Where(m => serviceIds.Contains(m.serviceId)).ToList();

                var totalCustomers = totals.Select(m => new
                {
                    service = db.Service.Find(m.serviceId),
                    customers = db.Customer.Where(x => m.customerIds.Contains(x.Id)),
                    count = m.count
                });


                List<ReportItem> repItems = new List<ReportItem>();

                foreach (var item in totalCustomers)
                {
                    var active = activeCustomer.FirstOrDefault(m => m.service.Id == item.service.Id);

                    ReportItem repItem = new ReportItem
                    {
                        Active = active != null ? active.count : 0,
                        NotActive = item.count - (active != null ? active.count : 0),
                        Total = item.count,
                        ActiveCustomers = active != null ? active.customers : null,
                        NotActiveCustomers = item.customers.ToList().Except(active != null ? active.customers.ToList() : null),
                        NewspaperName = item.service.NewsPaperName,
                    };
                    repItems.Add(repItem);
                }

                ReportItem repItemTotal = new ReportItem
                {
                    NewspaperName = "hDdf",
                    Active = repItems.Sum(m => m.Active),
                    NotActive = repItems.Sum(m => m.NotActive),
                    Total = repItems.Sum(m => m.Total)
                };

                repItems.Add(repItemTotal);
                rep.ReportItems = repItems;

                //End of ReportItems

                //Start of Thap Ghat
                var thap = (from s in db.ServiceAssign
                            from c in db.Customer
                            from p in db.Service
                            where s.CustomerId == c.Id && s.NewspaperId == p.Id
                           && s.PaperDispatchDate >= rep.StartDate && s.PaperDispatchDate <= rep.EndedDate
                            // && s.EndedDate >= rep.EndedDate
                            select new
                            {
                                customer = c,
                                ServiceAssign = s,
                                service = p
                            }).GroupBy(m => m.service.Id).Select(m => new
                            {
                                serviceId = m.Key,
                                count = m.Count(),
                                customerIds = db.ServiceAssign.Where(x => x.NewspaperId == m.Key && x.PaperDispatchDate >= rep.StartDate && x.PaperDispatchDate <= rep.EndedDate).Select(x => x.CustomerId)

                            }).ToList();

                thap = thap.Where(m => serviceIds.Contains(m.serviceId)).ToList();

                var thapCustomer = thap.Select(m => new
                {
                    service = db.Service.Find(m.serviceId),
                    customers = db.Customer.Where(x => m.customerIds.Contains(x.Id)),
                    count = m.count

                });

                var thapGhat = (from s in db.ServiceAssign
                                from c in db.Customer
                                from p in db.Service
                                where s.CustomerId == c.Id && s.NewspaperId == p.Id
                                 && s.PaperDispatchDate >= rep.StartDate && s.PaperDispatchDate <= rep.EndedDate
                                && s.EndedDate < rep.EndedDate
                                select new
                                {
                                    customer = c,
                                    ServiceAssign = s,
                                    service = p
                                }).GroupBy(m => m.service.Id).Select(m => new
                                {
                                    serviceId = m.Key,
                                    count = m.Count(),
                                    customerIds = db.ServiceAssign.Where(x => x.NewspaperId == m.Key && x.PaperDispatchDate >= rep.StartDate && x.PaperDispatchDate <= rep.EndedDate
                                && x.EndedDate < rep.EndedDate).Select(x => x.CustomerId)
                                }).ToList();

                thapGhat = thapGhat.Where(m => serviceIds.Contains(m.serviceId)).ToList();

                var thapGhatCustomer = thapGhat.Select(m => new
                {
                    service = db.Service.Find(m.serviceId),
                    customers = db.Customer.Where(x => m.customerIds.Contains(x.Id)),
                    count = m.count
                });


                List<ThapGhatItem> ThapGhatItems = new List<ThapGhatItem>();

                foreach (var item in thapCustomer)
                {
                    var GhatItem = thapGhatCustomer.FirstOrDefault(m => m.service.Id == item.service.Id);


                    ThapGhatItem thapGhatItem = new ThapGhatItem
                    {
                        NewspaperName = item.service.NewsPaperName,
                        Thap = item.customers.Count(),
                        Ghat = GhatItem != null ? GhatItem.customers.Count() : 0,
                        Total = 0
                    };

                    ThapGhatItems.Add(thapGhatItem);
                }

                ThapGhatItem thapGhatTotal = new ThapGhatItem
                {
                    NewspaperName = "hDdf",
                    Thap = ThapGhatItems.Sum(m => m.Thap),
                    Ghat = ThapGhatItems.Sum(m => m.Ghat)
                };

                ThapGhatItems.Add(thapGhatTotal);

                rep.ThapGhatItems = ThapGhatItems;
                //End of ThapGhat items


                //Start of Complement Customers
                List<ComplementItem> complementItems = new List<ComplementItem>();

                IEnumerable<int> ComplementOfficer = db.Officers.Where(m => m.OfficerType == "कम्प्लिमेन्ट").Select(m => m.Id).ToList();

                IEnumerable<int> ComplementCustomer = db.Customer.Where(m => ComplementOfficer.Contains(m.OfficerId.Value)).Select(m => m.Id).ToList();


                IEnumerable<ServiceAssign> complementServiceAssign = db.ServiceAssign.Where(s => s.PaperDispatchDate >= rep.StartDate
                                                && s.PaperDispatchDate <= rep.EndedDate && s.EndedDate >= rep.EndedDate && ComplementCustomer.Contains(s.CustomerId));

                int goPa = db.Service.FirstOrDefault(m => m.NewsPaperName.Trim() == "uf]/vfkq").Id;
                int raising = db.Service.FirstOrDefault(m => m.NewsPaperName.Trim() == "/fOlhË g]kfn").Id;
                int muna = db.Service.FirstOrDefault(m => m.NewsPaperName.Trim() == "d'gf").Id;
                int madhu = db.Service.FirstOrDefault(m => m.NewsPaperName.Trim() == "dw'ks{").Id;
                int yuwa = db.Service.FirstOrDefault(m => m.NewsPaperName.Trim() == "o'jfd~r").Id;

                IEnumerable<int> customerIds = complementServiceAssign.Select(m => m.CustomerId);


                foreach (var item in ComplementOfficer.ToList())
                {
                    IEnumerable<int> filteredCustomerIds = db.Customer.Where(m => customerIds.Contains(m.Id)
                                                            && m.OfficerId == item).Select(m => m.Id).ToList();
                    IEnumerable<ServiceAssign> FilteredAssign = complementServiceAssign.Where(m => filteredCustomerIds.Contains(m.CustomerId)).ToList();

                    ComplementItem compItem = new ComplementItem
                    {
                        Yuwa = FilteredAssign.Where(m => m.NewspaperId == yuwa) != null ?
                               FilteredAssign.Where(m => m.NewspaperId == yuwa).Count() : 0,
                        GoPa = FilteredAssign.Where(m => m.NewspaperId == goPa) != null ?
                               FilteredAssign.Where(m => m.NewspaperId == goPa).Count() : 0,
                        Muna = FilteredAssign.Where(m => m.NewspaperId == muna) != null ?
                               FilteredAssign.Where(m => m.NewspaperId == muna).Count() : 0,
                        Raising = FilteredAssign.Where(m => m.NewspaperId == raising) != null ?
                               FilteredAssign.Where(m => m.NewspaperId == raising).Count() : 0,
                        Madhu = FilteredAssign.Where(m => m.NewspaperId == madhu) != null ?
                               FilteredAssign.Where(m => m.NewspaperId == madhu).Count() : 0,
                        Total = FilteredAssign != null ? FilteredAssign.Count() : 0,

                        OfficerName = db.Officers.Find(item).Name
                    };

                    complementItems.Add(compItem);
                }

                ComplementItem totalComplementItem = new ComplementItem
                {
                    OfficerName = "hDdf",
                    GoPa = complementItems.Sum(m => m.GoPa),
                    Madhu = complementItems.Sum(m => m.Madhu),
                    Muna = complementItems.Sum(m => m.Muna),
                    Raising = complementItems.Sum(m => m.Raising),
                    Yuwa = complementItems.Sum(m => m.Yuwa),
                    Total = complementItems.Sum(m => m.Total)
                };
                complementItems.Add(totalComplementItem);
                rep.ComplementItems = complementItems;

                return View(rep);
            }
            return View(rep);
        }



    }
}
