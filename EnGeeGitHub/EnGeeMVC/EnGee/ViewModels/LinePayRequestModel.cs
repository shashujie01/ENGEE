namespace EnGee.ViewModels
{
    public class LinePayRequestModel
    {
        public int amount { get; set; }
        public string productImageUrl { get; set; }
        public string confirmUrl { get; set; }
        public string productName { get; set; }
        public string orderId { get; set; }
        public string currency { get; set; }
    }
}
