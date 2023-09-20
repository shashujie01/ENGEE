namespace EnGee.ViewModel
{
    public class JING_ProductPageViewModel
    {
        public List<JING_ProductViewModel> Products { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int? MainCategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public int? BrandId { get; set; }

        public string txtKeyword { get; set; }
    }
}
