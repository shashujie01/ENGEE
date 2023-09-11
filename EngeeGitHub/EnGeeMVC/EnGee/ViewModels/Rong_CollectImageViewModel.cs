using EnGee.Models;
using System.ComponentModel.DataAnnotations;

namespace EnGee.ViewModels
{
    public class Rong_CollectImageViewModel
    {
        private TCollect _collect;
        public Rong_CollectImageViewModel()
        {
            _collect = new TCollect();
        }
        public TCollect collect
        {
            get { return _collect; }
            set { _collect = value; }
        }

        public int CollectId
        {
            get { return _collect.CollectId; }
            set { _collect.CollectId = value; }
        }
        public int MemberId
        {
            get { return _collect.MemberId; }
            set { _collect.MemberId = value; }
        }
        public string CollectTitle
        {
            get { return _collect.CollectTitle; }
            set { _collect.CollectTitle = value; }
        }
        public string CollectCaption
        {
            get { return _collect.CollectCaption; }
            set { _collect.CollectCaption = value; }
        }
        public DateTime CollectEndDate
        {
            get { return _collect.CollectEndDate; }
            set { _collect.CollectEndDate = value; }
        }
        public int DeliveryTypeId
        {
            get { return _collect.DeliveryTypeId; }
            set { _collect.DeliveryTypeId = value; }
        }
        public string DeliveryAddress
        {
            get { return _collect.DeliveryAddress; }
            set { _collect.DeliveryAddress = value; }
        }
        public string ConvenienNum
        {
            get { return _collect.ConvenienNum; }
            set { _collect.ConvenienNum = value; }
        }

        public bool CollectStatus
        {
            get { return _collect.CollectStatus; }
            set { _collect.CollectStatus = value; }
        }

        public string? CollectImagePath
        {
            get { return _collect.CollectImagePath; }
            set { _collect.CollectImagePath = value; }
        }

        public IFormFile photo { get; set; }

        public string CollectItemName
        {
            get { return _collect.CollectItemName; }
            set { _collect.CollectItemName = value; }
        }
        public int MainCategoryId
        {
            get { return _collect.MainCategoryId; }
            set { _collect.MainCategoryId = value; }
        }

        public int SubcategoryId
        {
            get { return _collect.SubcategoryId; }
            set { _collect.SubcategoryId = value; }
        }
        public int CollectAmount
        {
            get { return _collect.CollectAmount; }
            set { _collect.CollectAmount = value; }
        }
        public DateTime CollectStartDate
        {
            get { return _collect.CollectStartDate; }
            set { _collect.CollectStartDate = value; }
        }

    }
}
