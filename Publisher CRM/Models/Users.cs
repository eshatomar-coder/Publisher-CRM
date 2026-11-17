using System;
using System.ComponentModel.DataAnnotations;

namespace Publisher_CRM.Models
{
    public class Users
    {
        public static object Identity { get; internal set; }
        [Key]
        public string RecordId { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }

        // Link the user to their specific role in the hierarchy
        public int? RoleId { get; set; }
       public virtual Role? Roles { get; set; } = null!;

        public DateTime? DateOfBirth { get; set; }
        public string? AddedById { get; set; }
        public string? ModifiedById { get; set; }
        public DateTime? AddedTime { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public string? Phone { get; set; }
        public string? Mobile { get; set; }
        public string? Website { get; set; }
        public string? Fax { get; set; }

        public string SalesCallAssign { get; set; }

        [Required(ErrorMessage = "Profile is required")]
        public string Profile { get; set; } // Kept required as you requested

        // Address Information
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? ZipCode { get; set; }

        public string? Password{ get; set; }

        // Preferences & Settings
        public string? Language { get; set; }
        public string? CountryLocale { get; set; }
        public string? TimeZone { get; set; }
        public string? TimeFormat { get; set; }
        public string? Currency { get; set; }
        public bool? ConfirmationStatus { get; set; }
        public string? Zuid { get; set; }
        public string? DateFormat { get; set; }
        public string? CurrentShiftId { get; set; }
        public string? NextShiftId { get; set; }
        public DateTime? ShiftEffectiveFrom { get; set; }
        public string? Grouping { get; set; }
        public string? Decimal { get; set; }
        public string? SortOrderPreference { get; set; }
        public string? NameFormat { get; set; }
        public string? Type { get; set; }
        public string? StatusReason { get; set; }
        public string? Source { get; set; }
        public string? PreferredUnitForDistance { get; set; }
        public string? SalespersonCode { get; set; }
        public string? PreferredCurrencyId { get; set; }
        public string? ReportingToId { get; set; }
        public string? UserStatus { get; set; }
    }
}