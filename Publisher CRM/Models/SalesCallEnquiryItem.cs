using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace Publisher_CRM.Models
{
    public class SalesCallEnquiryItem
    {
        [Key]
        public int Id { get; set; }

        // Links this item back to the main Sales Call
        [ForeignKey("Salescall")]
        public int SalesCallId { get; set; }
        public virtual Salescall Salescall { get; set; }

        public string? BrandName { get; set; }
        public string? SeriesName { get; set; }
        public string? Title { get; set; }
        public string? Isbn { get; set; }
        public string? Category { get; set; }

        public int Quantity { get; set; }
        public decimal TargetPrice { get; set; }
        public decimal OfferedPrice { get; set; }
        public decimal UnitCost { get; set; }

        // ADD THIS NEW FIELD FOR THE IMAGE:
        public string? CoverImageUrl { get; set; }

        public string? Specification { get; set; }
        public string? InternalRemark { get; set; }
        public string? ExternalRemark { get; set; }
    }
}