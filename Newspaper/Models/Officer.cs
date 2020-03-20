using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Newspaper.Models
{
    [Table("tblOfficer")]
    public class Officer
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Designation { get; set; }
        public string OfficeAddress { get; set; }
        public string Email { get; set; }
        public string PISNo { get; set; }
        public string Phone { get; set; }
        public bool Status { get; set; }
        public string OfficerType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}