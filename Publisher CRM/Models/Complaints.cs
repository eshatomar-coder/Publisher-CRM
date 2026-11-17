using System;
using System.ComponentModel.DataAnnotations;

namespace Publisher_CRM.Models
{
    public class Complaints
    {
        [Key]
        public int Id { get; set; }

        public string SalesCall { get; set; }

        public string AccountName { get; set; }

        public string? Status { get; set; }

        public string? SolutionRemarks { get; set; }

        // Using DateTime? allows the date to be empty/null initially
        [DataType(DataType.Date)]
        public DateTime? SolutionDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? SalesCallDate { get; set; }

        public string ComplaintRemark { get; set; }

        public string ComplaintsOwner { get; set; }
    }
}