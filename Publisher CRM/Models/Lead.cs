using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Publisher_CRM.Models
{
    public class LeadViewModel
    {
        [Key]
        public string RecordId { get; set; } = Guid.NewGuid().ToString();

        [Display(Name = "Enquiry ID")]
        public string? EnquiryId { get; set; }

        [Display(Name = "Sales Call ID")]
        public int? SalesCallId { get; set; }

        [Required(ErrorMessage = "Customer is mandatory for an Enquiry.")]
        [Display(Name = "Customer Name")]
        public string AccountNameId { get; set; }

        [Display(Name = "Contact Name")]
        public string? ContactNameId { get; set; }

        [Display(Name = "Enquiry Type")]
        public string? EnquiryType { get; set; }

        public string? Remarks { get; set; }

        [Display(Name = "Followup Date")]
        [DataType(DataType.Date)]
        public DateTime? FollowupDate { get; set; }

        [Display(Name = "Last Sales Call Date")]
        [DataType(DataType.Date)]
        public DateTime? LastSalesCallDate { get; set; }

        [Display(Name = "Last Call Remark")]
        public string? LastCallRemark { get; set; }

        [Display(Name = "Call Type")]
        public string? CallType { get; set; }

        public string? Nature { get; set; }

        [Display(Name = "Status")]
        public string? Stage { get; set; }

        [Display(Name = "Enquiry Value")]
        public decimal? EnquiryValue { get; set; }

        public string? Currency { get; set; }

        [Display(Name = "Commercial Terms")]
        public string? CommercialTerms { get; set; }

        // --- Title Management JSON (Stores Excel/Manual Data) ---
        [NotMapped]
        public string? EnquiryTitlesJson { get; set; }

        // --- System & Audit Info ---
        public string? LeadOwnerId { get; set; }
        public DateTime? CreatedTime { get; set; } = DateTime.Now;
        public DateTime? ModifiedTime { get; set; } = DateTime.Now;

        // --- Dropdown Lists (Not mapped to DB) ---
        [NotMapped] public IEnumerable<SelectListItem>? AccountsList { get; set; }
        [NotMapped] public IEnumerable<SelectListItem>? ContactsList { get; set; }
        [NotMapped] public IEnumerable<SelectListItem>? EnquiryTypesList { get; set; }
        [NotMapped] public IEnumerable<SelectListItem>? NaturesList { get; set; }
        [NotMapped] public IEnumerable<SelectListItem>? StatusList { get; set; }
        [NotMapped] public IEnumerable<SelectListItem>? CurrencyList { get; set; }
    }

    // Class to map the Excel/Manual Grid data
    public class EnquiryTitleItem
    {
        public string Isbn { get; set; }
        public string TitleName { get; set; }
        public int Qty { get; set; }
        public decimal NetPrice { get; set; }
    }
}