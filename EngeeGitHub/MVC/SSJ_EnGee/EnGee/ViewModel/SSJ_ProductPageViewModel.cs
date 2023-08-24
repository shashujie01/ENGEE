namespace Engee.ViewModel
{
    public class SSJ_ProductPageViewModel
    {
        public List<SSJ_ProductViewModel> Products { get; set; }
        public SSJ_ProductViewModel Product { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }

}
