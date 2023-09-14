using EnGee.Models;

namespace EnGee.ViewModels
{
    public class Rong_DonateViewModel
    {
        public int DonationOrderId { get; set; }

        public int CollectId { get; set; }

        public int MemberId { get; set; }

        public DateTime OrderDate { get; set; }
        public int DeliveryTypeId { get; set; }

        public string DeliveryType { get; set; }
        public string DeliveryAddress { get; set; }
        public int? DeliveryFee { get; set; }

        public string ConvenienNum { get; set; }
        public int CollectAmount { get; set; }
        public string CollectItemName { get; set; }
        public string DonarName { get; set; }
        public string DonarPhone { get; set; }
        public int DonationAmount { get; set; }
        public string DonationStatus { get; set; }
    }
}
