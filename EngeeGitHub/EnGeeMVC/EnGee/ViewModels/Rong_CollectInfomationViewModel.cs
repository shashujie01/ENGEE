using EnGee.Models;
using System.ComponentModel;

namespace EnGee.ViewModels
{
    public class Rong_CollectInfomationViewModel
    {
    //    private TCollect _collect;
    //    public TCollect collect
    //    {
    //        get { return _collect; }
    //        set { _collect = value; }
    //    }
    //    public Rong_CollectInfomationViewModel() 
    //    {
    //        _collect = new TCollect();
    //    }

        public int CollectId { get; set; }
        public string Nickname { get; set; }
        public string CollectTitle { get; set; }
        public string CollectCaption
        { get; set; }
        public string CollectStartDate { get; set; }

        public string CollectEndDate { get; set; }
        public int DeliveryTypeId { get; set; }
        public string DeliveryType { get; set; }
        public string DeliveryAddress { get; set; }
        public string ConvenienNum { get; set; }
        public string? CollectImagePath { get; set; }
        public string? CollectItemName { get; set; }
        public int? CollectAmount { get; set; }

        //[DisplayName("上架中")]
        //public bool CollectStatus { get; set; }

    }
}
