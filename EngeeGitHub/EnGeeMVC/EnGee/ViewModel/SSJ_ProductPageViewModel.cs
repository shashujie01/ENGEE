namespace EnGee.ViewModel
{
    public class SSJ_ProductPageViewModel
    {//專為IndexSSJ頁面切換、Filter的顯示使用資料
        public List<SSJ_ProductViewModel> Products { get; set; }
        //頁面切換需使用ProductViewModel與foreach使用
        public int CurrentPage { get; set; }//切換頁面使用
        public int TotalPages { get; set; }//切換頁面使用
        public int? MainCategoryId { get; set; }//Categories Filter
        public int? SubCategoryId { get; set; }//Categories Filter
        public int? BrandId { get; set; }//Categories Filter
        public string? TxtKeyword { get; set; }//關鍵字 Filter
    }
}
