namespace SportStore.ViewModels.ProductVM
{
    public class ProductEditViewModel : ProductCreateViewModel
    {
        public long? ProductID { get; set; }

        public string? ExistingPhotoPath { get; set; }
    }
}
