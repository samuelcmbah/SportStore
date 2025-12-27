namespace SportStore.Models.ViewModels
{
    public class ProductEditViewModel : ProductCreateViewModel
    {
        public long? ProductID { get; set; }

        public string? ExistingPhotoPath { get; set; }
    }
}
