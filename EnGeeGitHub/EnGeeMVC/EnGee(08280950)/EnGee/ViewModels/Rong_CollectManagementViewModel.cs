using System.ComponentModel;

namespace EnGee.ViewModels
{
    public class Rong_CollectManagementViewModel
    {
        [DisplayName("文章編號")]
        public int CollectId { get; set; }
        [DisplayName("會員編號")]
        public int MemberId { get; set; }
        [DisplayName("文章標題")]
        public string CollectTitle { get; set; }
        [DisplayName("募集開始日期")]
        public string CollectStartDate { get; set; }

        [DisplayName("募集結束日期")]
        public string CollectEndDate { get; set; }
        [DisplayName("提供之寄送方式")]
        public string DeliveryType { get; set; }
        [DisplayName("上架中")]
        public bool CollectStatus { get; set; }
    }
}