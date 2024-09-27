namespace EdsanBooking.Models
{
    public class UploadImagesViewModel
    {
        public string FeatureName { get; set; }
        public string TypeName { get; set; }
        public List<IFormFile> ImageFiles { get; set; }
    }
}
