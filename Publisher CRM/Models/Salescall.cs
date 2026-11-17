using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Publisher_CRM.Models
{
    public class Salescall
    {
        [Key]
        public int Id { get; set; }

        public string? SalesCallName { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CallDateTime { get; set; }

        // Removed [Required] so Physical Events can have "New Customers" (No Account ID)
        public string? AccountName { get; set; }
        public string? Contacts { get; set; }
        public string? Associates { get; set; } // Added for Multiple Associates

        public string? Owner { get; set; }
        public string? TeamMember { get; set; }

        // Call Flow Routing
        public string? CallType { get; set; }    // Virtual, Physical, Phone
        public string? CallSubType { get; set; } // Enquiry, Order, Complaint, Merchandising

        public int? EventId { get; set; }        // Links to EventMaster if Physical + Event

        public string? Pipeline { get; set; }

        public bool MerchandisingCheck { get; set; }
        public bool IsComplaint { get; set; }

        [NotMapped]
        public List<IFormFile>? UploadedFiles { get; set; }

        public string? CustomerRelationship { get; set; }

        // --- Demographics ---
        public string? CustomerChannel { get; set; }
        public string? CustomerActivity { get; set; }
        public string? CustomerGeography { get; set; }
        public string? CustomerZone { get; set; }
        public string? CustomerProductProfile { get; set; }

        // --- Orders ---
        public bool PotentialOrder { get; set; }
        public bool SampleRequired { get; set; }
        public bool HasOrder { get; set; }
        public string? OrderType { get; set; }

        // --- Follow Up & Remarks ---
        [DataType(DataType.Date)]
        public DateTime? FollowUpDate { get; set; }

        public string? Remarks { get; set; }

        public virtual ICollection<SalesCallEnquiryItem>? EnquiryItems { get; set; }
    }
}