using System;
using System.ComponentModel.DataAnnotations;

namespace Publisher_CRM.Models
{
    public class Files
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string ModuleName { get; set; } // e.g., "SalesCall", "Lead", "Account", "OrderManagement", "Contact"

        [Required]
        [MaxLength(100)]
        public string RecordId { get; set; } // The ID of the specific record (SalesCall ID, Lead RecordId, etc.)

        [Required]
        public string FilePath { get; set; } // The actual saved path (e.g., /Uploads/SalesCalls/file.pdf)

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } // Original file name for display purposes

        public DateTime UploadedDate { get; set; } = DateTime.Now;
    }
}

