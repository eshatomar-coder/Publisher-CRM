using System.ComponentModel.DataAnnotations;

namespace Publisher_CRM.Models
{
    public class RetailerPointview
    {
        [Key]
        public int Id { get; set; }

        // --- Core Information ---
        public string? RPCode { get; set; }
        public string? AccountCode { get; set; }
        public string? RetailerName { get; set; }
        public string? OwnerName { get; set; }
        public string? MobileNumber { get; set; }
        public string? EmailAddress { get; set; }
        public string? GoogleMapLocation { get; set; }
        public string? Address { get; set; }

        // --- Store Details ---
        public string? Category { get; set; }
        public string? CategoryOther { get; set; } // For "Other: ____"
        public string? StoreSize { get; set; }
        public string? StoreLocationQuality { get; set; }
        public string? StoreLocationOther { get; set; } // For "Other: ____"
        public string? EstimatedFootfall { get; set; }
        public string? CustomerProfile { get; set; }

        // --- Current Inventory & Fit ---
        public string? CurrentlySells { get; set; }
        public string? CurrentlySellsOther { get; set; } // For "Other: ____"
        public string? ShareOfChildrensProducts { get; set; }
        public string? PegasusProductFit { get; set; }
        public string? PegasusProductFitOther { get; set; } // For "Other: ____"
        public string? OverallPegasusFit { get; set; }
        public string? ReasonForOverallFit { get; set; }
        public string? IsPegasusAvailable { get; set; }
        public string? SourceOfSupply { get; set; }

        // --- Buying Pattern ---
        public string? BuyingPattern { get; set; }
        public string? PurchaseFrequency { get; set; }
        public string? MonthlyPurchaseValue { get; set; }

        // --- Display & Visibility ---
        public string? DisplaySpaceAvailable { get; set; }
        public string? SuitableDisplayType { get; set; }
        public string? WillingnessToGiveVisibility { get; set; }
        public string? VisibilityRemarks { get; set; }
        public string? MerchandisingSupport { get; set; }

        // --- Interest & Objections ---
        public string? RetailerInterest { get; set; }
        public string? ReasonForInterest { get; set; }
        public string? MainObjection { get; set; }
        public string? MainObjectionOther { get; set; } // For "Other: ____"

        // --- Assessment & Grading ---
        public string? GradeTheRetailer { get; set; }
        public string? ReasonForGrade { get; set; }
        public string? RecommendedAction { get; set; }

        // Single Text Fields
        public string? KeyObservations { get; set; }
        public string? Risks { get; set; }
        public string? Opportunities { get; set; }
        public string? FinalRecommendation { get; set; }

        // --- NSM Review ---
        public string? ReviewedByNSM { get; set; }
        public string? NSMDecision { get; set; }
        public string? NSMRemarks { get; set; }
        public string? FinalRetailerStatus { get; set; }
    }
}