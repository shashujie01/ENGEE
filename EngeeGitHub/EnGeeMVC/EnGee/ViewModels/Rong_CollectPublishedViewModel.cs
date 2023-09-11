using EnGee.Models;
using System;
using System.ComponentModel;

namespace EnGee.ViewModels
{
    public class Rong_CollectPublishedViewModel
    {
        public int CollectId { get; set; }
        [DisplayName("許願標題")]
        public string CollectTitle { get; set; }
        [DisplayName("募集需求描述")]
        public string CollectCaption { get; set; }
        [DisplayName("結束")]
        public DateTime CollectEndDate { get; set; }
        //[DisplayName("許願標題")]
        //public string CollectImagePath { get; set; }
        public string CollectItemName { get; set; }
        public int MainCategoryId { get; set; }
        public int SubcategoryId { get; set; }
        public int CollectAmount { get; set; }
        public int DeliveryTypeId { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? ConvenienNum { get; set; }
        // 上架狀態皆預設為上架中
        public bool CollectStatus { get; set; }
        public List<TCollectItem> CollectItems { get; set; }

    }
}
