using EnGee.Models;
using System.ComponentModel;

namespace EnGee.ViewModels
{
    public class Rong_CollectInformationViewModel
    {

        public int CollectId { get; set; }
        public int MemberId { get; set; }
        public string Username { get; set; }
        public string CollectTitle { get; set; }
        public string CollectCaption
        { get; set; }
        public string CollectStartDate { get; set; }

        public string CollectEndDate { get; set; }
        public int DeliveryTypeId { get; set; }
        public string DeliveryType { get; set; }
        public int DeliveryFee { get; set; }
        public string DeliveryAddress { get; set; }
        public string ConvenienNum { get; set; }
        public string? CollectImagePath { get; set; }
        public string? CollectItemName { get; set; }
        public int? CollectAmount { get; set; }

        public TMember member { get; set; }
        //public TCollect collect { get; set; }

    }
}
