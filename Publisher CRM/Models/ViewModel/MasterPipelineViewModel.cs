using System;

namespace Publisher_CRM.Models.ViewModels
{
    public class MasterPipelineViewModel
    {
        public int SeriesId { get; set; }
        public string SeriesIdString { get; set; }
        public string SeriesName { get; set; }
        public string BrandName { get; set; }
        public string CategoryName { get; set; }

        public int TotalTitles { get; set; }
        public int ClosedTitles { get; set; }
        public int CompletionPercentage { get; set; }

        public string ContentStatus { get; set; }
        public string ContentTeam { get; set; } // NEW
        public string ContentOwner { get; set; }
        public int TotalContentRecords { get; set; }

        public string AssetStatus { get; set; }
        public string AssetTeam { get; set; } // NEW
        public string AssetOwner { get; set; }
        public int TotalAssetsUploaded { get; set; }

        public int TotalCataloguesExported { get; set; }
        public string AllTitleNames { get; set; }
    }

    public class TitleQueryViewModel
    {
        public string SeriesName { get; set; }
        public List<TitleQueryItem> Items { get; set; } = new List<TitleQueryItem>();
    }

    public class TitleQueryItem
    {
        public int TitleId { get; set; }
        public string TitleName { get; set; }
        public string ISBN { get; set; }
        public int AssetsUploaded { get; set; }
        public bool HasContent { get; set; }
        public int CataloguesExported { get; set; }
        public int ReadinessPercentage { get; set; }

        // --- ADD THESE TWO NEW PROPERTIES ---
        public string ProductStatus { get; set; }
        public DateTime? PublishingDate { get; set; }
    }
}