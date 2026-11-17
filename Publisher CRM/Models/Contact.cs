using System;
using System.ComponentModel.DataAnnotations;

namespace Publisher_CRM.Models
{
    // Removed the [Index] attribute here because the ApplicationDbContext handles it with a filter
    public class Contact
    {
        [Key]
        public int ContactId { get; set; }

        // --- System & Tracking Identifiers ---
        [Display(Name = "Record ID")]
        public string? RecordId { get; set; }

        [Display(Name = "Contact Owner ID")]
        public string? ContactOwnerId { get; set; }

        public string? LayoutId { get; set; }

        // --- Contact Information ---
        public string? Salutation { get; set; }

        public string? AccountNameId { get; set; } // For the dropdown linkage

        [Display(Name = "Account Name")]
        public string? AccountName { get; set; }

        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is mandatory.")]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Display(Name = "Contact Name")]
        public string? ContactName { get; set; }

        public string? Title { get; set; }
        public string? Department { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? Phone { get; set; }

        [Display(Name = "Other Phone")]
        public string? OtherPhone { get; set; }

        public string? Mobile { get; set; }

        [Display(Name = "Sales Call")]
        public int? SalesCall { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Salesperson Code")]
        public string? SalespersonCode { get; set; }

        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }

        [Display(Name = "Modified By")]
        public string? ModifiedBy { get; set; }

        [Display(Name = "Connected To")]
        public string? ConnectedTo { get; set; }

        [Display(Name = "Social Lead ID")]
        public string? SocialLeadId { get; set; }

        public string? Description { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Joining Date")]
        public DateTime? JoiningDate { get; set; }

        [Display(Name = "Contact Owner")]
        public string? ContactOwner { get; set; }

        public string? Pipeline { get; set; }

        [Display(Name = "Customer ERP Code")]
        public string? CustomerErpCode { get; set; }

        // --- Timestamps & System Dates ---
        [Display(Name = "Created Time")]
        public DateTime? CreatedTime { get; set; }

        [Display(Name = "Modified Time")]
        public DateTime? ModifiedTime { get; set; }

        [Display(Name = "Last Activity Time")]
        public DateTime? LastActivityTime { get; set; }

        [Display(Name = "User Modified Time")]
        public DateTime? UserModifiedTime { get; set; }

        [Display(Name = "User Related Activity Time")]
        public DateTime? UserRelatedActivityTime { get; set; }

        [Display(Name = "System Modified Time")]
        public DateTime? SystemModifiedTime { get; set; }

        // --- Web & Visitor Analytics ---
        [Display(Name = "Most Recent Visit")]
        public DateTime? MostRecentVisit { get; set; }

        [Display(Name = "First Page Visited")]
        public string? FirstPageVisited { get; set; }

        [Display(Name = "Average Time Spent (Minutes)")]
        public double? AverageTimeSpentMinutes { get; set; }

        [Display(Name = "Number Of Chats")]
        public int? NumberOfChats { get; set; }

        public string? Referrer { get; set; }

        [Display(Name = "Visitor Score")]
        public double? VisitorScore { get; set; }

        [Display(Name = "First Visit")]
        public DateTime? FirstVisit { get; set; }

        [Display(Name = "Days Visited")]
        public int? DaysVisited { get; set; }

        [Display(Name = "BC Contact No")]
        public string? BCContactNo { get; set; }
    }
}