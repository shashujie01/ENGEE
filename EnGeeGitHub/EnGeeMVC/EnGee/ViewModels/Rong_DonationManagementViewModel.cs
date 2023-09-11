using System.ComponentModel;

namespace EnGee.ViewModels
{
    public class Rong_DonationManagementViewModel
    {
        [DisplayName("訂單編號")]
        public int DonationOrderId { get; set; }
        [DisplayName("會員編號")]
        public int MemberId { get; set; }
        [DisplayName("許願編號")]
        public int CollectId { get; set; }
        [DisplayName("訂單時間")]
        public string OrderDate { get; set; }

        [DisplayName("運送")]
        public string DeliveryType { get; set; }
        [DisplayName("姓名")]
        public string DonarName { get; set; }
        [DisplayName("電話")]
        public string DonarPhone { get; set; }
        [DisplayName("品項")]
        public string CollectItemName { get; set; }
        [DisplayName("捐贈數量")]
        public int DonationAmount { get; set; }
        [DisplayName("訂單狀態")]
        public string DonationStatus { get; set; }

    }
}
