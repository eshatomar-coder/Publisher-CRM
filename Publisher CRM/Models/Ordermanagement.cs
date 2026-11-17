using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Publisher_CRM.Models
{
    public class OrderManagementViewModel
    {
        [Key]
        public string RecordId { get; set; } = Guid.NewGuid().ToString();

        [Display(Name = "Order ID")]
        public string? OrderId { get; set; }

        [Display(Name = "Sales Call ID")]
        public int? SalesCallId { get; set; }

        [Required(ErrorMessage = "Customer Name is required.")]
        [Display(Name = "Customer Name")]
        public string AccountNameId { get; set; }

        [Display(Name = "Contact Name")]
        public string? ContactNameId { get; set; }

        [Display(Name = "Order Type")]
        public string? OrderType { get; set; } // OEM, White Labeling, Distribution

        [Display(Name = "Status")]
        public string? Stage { get; set; } // Open, In Working, Closed

        [Display(Name = "Tentative Dispatch Date")]
        [DataType(DataType.Date)]
        public DateTime? TentativeDispatchDate { get; set; }

        [Display(Name = "Order Value")]
        public decimal? OrderValue { get; set; }

        public string? Currency { get; set; }

        [Display(Name = "Commercial Terms")]
        public string? CommercialTerms { get; set; }

        public string? Remarks { get; set; }

        // --- BC ERP Integration ---
        [Display(Name = "BC SO No.")]
        public string? BcSoNo { get; set; }

        [Display(Name = "BC SO Date")]
        [DataType(DataType.Date)]
        public DateTime? BcSoDate { get; set; }

        // --- Project Management ---
        [Display(Name = "Title Status")]
        public string? TitleStatus { get; set; } // Open, Created

        [Display(Name = "Project Parked")]
        public bool ProjectParked { get; set; }

        [Display(Name = "Customer Project Coordinator")]
        public string? CustomerProjectCoordinatorId { get; set; } // Contact ID

        // --- File Attachments ---
        [NotMapped]
        [Display(Name = "Agreement / PO")]
        public IFormFile? AgreementPoFile { get; set; }
        public string? AgreementPoFilePath { get; set; }

        // --- JSON Grid for Complex OEM/WL Titles ---
        [NotMapped]
        public string? OrderTitlesJson { get; set; }

        // --- System Data ---
        public string? OrderManagementOwnerId { get; set; }
        public DateTime? CreatedTime { get; set; } = DateTime.Now;
        public DateTime? ModifiedTime { get; set; } = DateTime.Now;

        // --- UI Dropdowns ---
        [NotMapped] public IEnumerable<SelectListItem>? OwnersList { get; set; }
        [NotMapped] public IEnumerable<SelectListItem>? AccountsList { get; set; }
        [NotMapped] public IEnumerable<SelectListItem>? OrderTypesList { get; set; }
        [NotMapped] public IEnumerable<SelectListItem>? StatusList { get; set; }
        [NotMapped] public IEnumerable<SelectListItem>? TitleStatusList { get; set; }
        [NotMapped] public IEnumerable<SelectListItem>? CurrencyList { get; set; }
    }

    // Maps the heavy OEM/WL Title Master grid
    public class OrderTitleItem
    {
        public string Type { get; set; } // OEM, WL, Dist
        public string Isbn { get; set; }
        public string PrimaryItemNo { get; set; }
        public string PrimaryItemName { get; set; }
        public string TitleName { get; set; }
        public string Language { get; set; }
        public string TechSpecs { get; set; }
        public string Inclusions { get; set; }
        public int Pages { get; set; }
        public string Book { get; set; }
        public string Toy { get; set; }
        public string Adaptations { get; set; }
        public string IsbnIssuer { get; set; } // Customer / Company
        public string BcCode { get; set; }
        public int Qty { get; set; }
        public decimal NetPrice { get; set; }
    }

    // --- BC API Mapping Classes ---
    public class BcODataResponse
    {
        public List<BcSalesOrder> value { get; set; }
    }

    public class BcSalesOrder
    {
        public string No { get; set; }
        public string Sell_to_Customer_No { get; set; }
        public string Sell_to_Customer_Name { get; set; }
        public string Sell_to_Contact { get; set; }
        public string Currency_Code { get; set; }
        public decimal Amount { get; set; }
        public DateTime? Document_Date { get; set; }
        public string Status { get; set; }

        // NEW: Used to map the Owner
        public string Salesperson_Code { get; set; }
    }

}