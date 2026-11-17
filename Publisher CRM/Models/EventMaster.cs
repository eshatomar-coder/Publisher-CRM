using System;
using System.ComponentModel.DataAnnotations;
namespace Publisher_CRM.Models
{
    public enum EventStatus
    {
        None,
        Open,
        Closed
    }
    public class EventMaster
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Event Name")]
        public string EventName { get; set; }

        // This will store the ID of the user selected from the dropdown
        [Display(Name = "Event Master Owner")]
        public string? EventMasterOwnerId { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Event Date From")]
        public DateTime? EventDateFrom { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Event Date To")]
        public DateTime? EventDateTo { get; set; }

        [Display(Name = "Event Location")]
        public string? EventLocation { get; set; }

        [Display(Name = "Number Of Customer Met")]
        public int? NumberOfCustomerMet { get; set; }

        [Display(Name = "Salesperson Code")]
        public string? SalespersonCode { get; set; }

        [Display(Name = "Event Status")]
        public EventStatus EventStatus { get; set; }

        // Costs
        [DataType(DataType.Currency)]
        [Display(Name = "Rental Charges")]
        public decimal? RentalCharges { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Travel Costs")]
        public decimal? TravelCosts { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Designing & Setup Charges")]
        public decimal? DesigningAndSetupCharges { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Other Costs")]
        public decimal? OtherCosts { get; set; }

        // --- HIDDEN/SYSTEM FIELDS ---

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Event Close Date")]
        public DateTime? EventCloseDate { get; set; }
    }

}

