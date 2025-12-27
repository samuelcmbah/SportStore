namespace SportStore.ViewModels.ProductVM
{
    public class PagingInfoViewModel
    {
        public int TotalItems { get; set; }

        public int ItemsPerPage { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages => (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);

        public int NextPage => CurrentPage < TotalPages ? CurrentPage + 1 : 1;

        public int PrevPage => CurrentPage > 1 ? CurrentPage - 1 : TotalPages; 
    }
}
