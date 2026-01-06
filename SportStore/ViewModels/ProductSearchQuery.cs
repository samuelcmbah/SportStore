namespace SportStore.ViewModels
{
    public class ProductSearchQuery
    {
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public bool IncludeInactive { get; set; } // useful for admin later
    }
}
