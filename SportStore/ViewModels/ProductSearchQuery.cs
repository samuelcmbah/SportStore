namespace SportStore.ViewModels
{
    public class ProductSearchFilterQuery
    {
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public bool IncludeInactive { get; set; } // useful for admin later
    }
}
