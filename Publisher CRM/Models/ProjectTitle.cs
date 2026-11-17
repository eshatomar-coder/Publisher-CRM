using System;
using System.ComponentModel.DataAnnotations;

namespace Publisher_CRM.Models
{
    public class ProjectTitle
    {
        [Key]
        public string RecordId { get; set; } = Guid.NewGuid().ToString();

        [Display(Name = "Title ID")]
        public string TitleId { get; set; }

        [Display(Name = "Order Record ID")]
        public string? OrderRecordId { get; set; }

        public string? CustomerId { get; set; }
        public string? CustomerName { get; set; }

        public string? Isbn { get; set; }

        [Display(Name = "Enquiry Type")]
        public string? Type { get; set; } // OEM, White Labeling, Distribution

        public string? PrimaryItemNo { get; set; }
        public string? PrimaryItemName { get; set; }

        public string? TitleName { get; set; }
        public string? Language { get; set; }

        [Display(Name = "Status")]
        public string Stage { get; set; } = "Open"; // Workflow Status Engine

        public string? Remarks { get; set; }

        [DataType(DataType.Date)]
        public DateTime? TentativeDispatchDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FileRecdDate { get; set; }

        public string? OwnerId { get; set; }
        public DateTime? CreatedTime { get; set; } = DateTime.Now;
        public DateTime? ModifiedTime { get; set; } = DateTime.Now;
    }
}