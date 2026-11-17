using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Publisher_CRM.Models
{
    // --- 1. BEAT ROUTE ---
    public class BeatRoute
    {
        [Key]
        public int BeatRouteId { get; set; }

        [Required]
        [Display(Name = "Beat Route Name")]
        public string RouteName { get; set; }

        [Required]
        [Display(Name = "Sales Person")]
        public string SalesPersonId { get; set; } // Links to Users.RecordId
        [ForeignKey("SalesPersonId")]
        public virtual Users? SalesPerson { get; set; }

        // Navigation property for mapped customers
        public virtual ICollection<BeatRouteCustomer> RouteCustomers { get; set; } = new List<BeatRouteCustomer>();

        [NotMapped]
        public int NumberOfCustomers => RouteCustomers?.Count ?? 0;
    }

    // --- 2. BEAT ROUTE CUSTOMER MAPPING ---
    public class BeatRouteCustomer
    {
        [Key]
        public int Id { get; set; }

        public int BeatRouteId { get; set; }
        [ForeignKey("BeatRouteId")]
        public virtual BeatRoute? BeatRoute { get; set; }

        public int AccountId { get; set; } // Links to Account (Customer)
        [ForeignKey("AccountId")]
        public virtual Account? Customer { get; set; }
    }

    // --- 3. BEAT PLAN ---
    public class BeatPlan
    {
        [Key]
        public int BeatPlanId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime PlanDate { get; set; }

        [Required]
        public int BeatRouteId { get; set; }
        [ForeignKey("BeatRouteId")]
        public virtual BeatRoute? BeatRoute { get; set; }

        public string? City { get; set; }

        [Required]
        public string SalesPersonId { get; set; } // Links to Users.RecordId
        [ForeignKey("SalesPersonId")]
        public virtual Users? SalesPerson { get; set; }

        public string? Associates { get; set; } // Comma separated or text

        // Navigation property for scheduled meetings
        public virtual ICollection<PlanMeeting> PlanMeetings { get; set; } = new List<PlanMeeting>();

        [NotMapped]
        public int NumberOfMeetings => PlanMeetings?.Count ?? 0;
    }

    // --- 4. PLAN MEETING ---
    public class PlanMeeting
    {
        [Key]
        public int PlanMeetingId { get; set; }

        public int BeatPlanId { get; set; }
        [ForeignKey("BeatPlanId")]
        public virtual BeatPlan? BeatPlan { get; set; }

        public int AccountId { get; set; } // The Customer
        [ForeignKey("AccountId")]
        public virtual Account? Customer { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan MeetingTime { get; set; }

        public string? Remarks { get; set; }
    }

    public class DailyPlan
    {
        [Key]
        public int DailyPlanId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string CallType { get; set; } // Virtual, Physical, Phone
        public string Place { get; set; }    // Link, Event/Customer/Company, or Phone No

        public string SalesPersonId { get; set; }
        [ForeignKey("SalesPersonId")]
        public virtual Users? SalesPerson { get; set; }

        public string Associates { get; set; } // Comma-separated user names

        public string CreatedById { get; set; }
        [ForeignKey("CreatedById")]
        public virtual Users? CreatedBy { get; set; }

        public virtual ICollection<DailyPlanCustomer> Customers { get; set; } = new List<DailyPlanCustomer>();
        public virtual ICollection<DailyPlanContact> Contacts { get; set; } = new List<DailyPlanContact>();
    }

    public class DailyPlanCustomer
    {
        [Key]
        public int Id { get; set; }
        public int DailyPlanId { get; set; }
        [ForeignKey("DailyPlanId")]
        public virtual DailyPlan? DailyPlan { get; set; }

        public int AccountId { get; set; }
        [ForeignKey("AccountId")]
        public virtual Account? Customer { get; set; }
    }

    public class DailyPlanContact
    {
        [Key]
        public int Id { get; set; }
        public int DailyPlanId { get; set; }
        [ForeignKey("DailyPlanId")]
        public virtual DailyPlan? DailyPlan { get; set; }

        public int ContactId { get; set; }
        [ForeignKey("ContactId")]
        public virtual Contact? Contact { get; set; }
    }
}