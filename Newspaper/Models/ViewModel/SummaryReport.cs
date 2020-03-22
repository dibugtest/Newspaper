using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Newspaper.Models.ViewModel
{
    public class SummaryReport
    {
        public string StartNepDate { get; set; }
        public string EndNepDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndedDate { get; set; }
        public IEnumerable<ReportItem> ReportItems { get; set; }
        public IEnumerable<ThapGhatItem> ThapGhatItems { get; set; }
        public IEnumerable<ComplementItem> ComplementItems { get; set; }

    }
    public class ReportItem {
        public string NewspaperName { get; set; }
        public int Total { get; set; }
        public int Active { get; set; }
        public int NotActive { get; set; }
        public IEnumerable<Customer> ActiveCustomers { get; set; }
        public IEnumerable<Customer> NotActiveCustomers { get; set; }
    }

    public class ThapGhatItem {
        public string NewspaperName { get; set; }
        public int Thap { get; set; }
        public int Ghat { get; set; }
        public int Total { get; set; }
    }

    public class ComplementItem
    {
        public string OfficerName { get; set; }
        public int GoPa { get; set; }
        public int Raising { get; set; }
        public int Muna { get; set; }
        public int Madhu { get; set; }
        public int Yuwa { get; set; }
        public int Total { get; set; }

    }
}