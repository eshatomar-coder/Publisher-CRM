using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration.Attributes;

namespace Publisher_CRM.Models
{
    public class Account
    {
        [Key]
        [Ignore]
        public int AccountId { get; set; }
        public string? RecordId { get; set; }

        // ==========================================
        // POSTCODE MAPPING LOGIC
        // ==========================================
        [Ignore]
        public int? PostCodeId { get; set; }

        [ForeignKey("PostCodeId")]
        [Ignore]
        public virtual Postcode PostCode { get; set; }

        [NotMapped]
        [Name("Postcode.id", "Postcodeid", "Postcode Id")]
        public string? CsvPostcodeId { get; set; }

        // ==========================================
        // Core Account Details
        // ==========================================
        [Name("AccountOwner", "Account Owner")]
        public string? AccountOwner { get; set; }

        [Required]
        public string AccountName { get; set; }

        public string? ContactName { get; set; } // NEW
        public string? Phone { get; set; }
        public string? CompanyName { get; set; }
        public string? Mobile { get; set; }
        public string? Email { get; set; }

        // Address Details
        public string? Address { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }

        [Name("Postcode", "Post Code", "Zip", "Zip Code", "ZipCode")]
        public string? Postcode { get; set; }
        public string? Country { get; set; }

        // Custom CRM Data
        public string? Status { get; set; } // NEW: Potential, Active, In-Active, Dormant, New, Closed
        public string? AccountStatus { get; set; } // NEW: Hold, Pending, Approved
        public string? Source { get; set; } // NEW

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TwelveMonthBusiness { get; set; } // NEW

        public string? CustomerRelationship { get; set; }
        public string? Description { get; set; }
        public bool? NoOutputAccount { get; set; }

        // INTERACTION COUNTS
        public int? SalesCallCount { get; set; }
        public int? RemarkCount { get; set; }
        public int? IssueCount { get; set; }
        public int? EnquiryCount { get; set; }
        public int? OrderCount { get; set; }

        // PROFILE SECTION
        public string? Website { get; set; }
        public int? NoBranchesOutlets { get; set; }
        public string? PanNo { get; set; }
        public string? CompanyRegNoCIN { get; set; }
        public string? GSTRegistrationNo { get; set; }
        public string? YearlyBusinessExpected { get; set; }
        public int? YearsInOperation { get; set; }
        public string? StateCode { get; set; }
        public string? FrequencyOfOrder { get; set; }
        public decimal? InitialOrderValue { get; set; }
        public string? PaymentTerm { get; set; }
        public decimal? Balance { get; set; }
        public string? DivisionCode { get; set; }

        public string? CustomerChannel { get; set; }
        public string? CustomerActivity { get; set; }
        public string? CustomerGeography { get; set; }
        public string? CustomerProductProfile { get; set; }
        public string? CustomerZone { get; set; }

        // INTERNAL INFORMATION SECTION
        public string? ERPCustomerCode { get; set; }
        public string? Location { get; set; }
        public string? ERPStatus { get; set; }
        public string? SalespersonCode { get; set; }

        [Display(Name = "BC Account No")]
        public string? BCAccountNo { get; set; }
    }
}