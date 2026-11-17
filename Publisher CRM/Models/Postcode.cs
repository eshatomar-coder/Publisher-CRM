using System;
using System.ComponentModel.DataAnnotations;
using CsvHelper.Configuration.Attributes;

namespace Publisher_CRM.Models
{
    public class Postcode
    {
        [Key]
        [Ignore] // Tells CsvHelper not to look for this in the CSV
        public int Id { get; set; } // Database Primary Key

        // ==========================================
        // CRM CORE IDENTIFIERS
        // ==========================================
        [Name("Record Id", "RecordId")]
        public string? RecordId { get; set; }

        [Name("Record Status", "RecordStatus")]
        public string? RecordStatus { get; set; }

        [Name("Master Postcodes Owner.id", "OwnerId")]
        public string? OwnerId { get; set; }

        // ==========================================
        // LOCATION DATA
        // ==========================================
        [Name("Zipcodes", "ZipCode", "Zip Code")]
        public string? ZipCode { get; set; }

        public string? City { get; set; }
        public string? State_Name { get; set; }
        public string? State_Code { get; set; }

        // ==========================================
        // CONTACT INFO
        // ==========================================
        public string?Email { get; set; }

        [Name("Secondary Email", "SecondaryEmail")]
        public string? SecondaryEmail { get; set; }

        [Name("Email Opt Out")]
        public bool?EmailOptOut { get; set; }

        // ==========================================
        // TRACKING & TIMESTAMPS
        // ==========================================
        [Name("Created By.id")]
        public string? CreatedById { get; set; }

        [Name("Modified By.id")]
        public string? ModifiedById { get; set; }

        [Name("Created Time")]
        public DateTime? CreatedTime { get; set; }

        [Name("Modified Time")]
        public DateTime? ModifiedTime { get; set; }

        // ==========================================
        // FINANCIALS & SYSTEM METADATA
        // ==========================================
        public string? Currency { get; set; }

        [Name("Exchange Rate")]
        public decimal? ExchangeRate { get; set; }

        [Name("Layout.id", "LayoutId")]
        public string? LayoutId { get; set; }

        public string? Tag { get; set; }

        public bool?Locked { get; set; }

        [Name("Is Record Duplicate", "IsRecordDuplicate")]
        public bool?IsRecordDuplicate { get; set; }
    }
}