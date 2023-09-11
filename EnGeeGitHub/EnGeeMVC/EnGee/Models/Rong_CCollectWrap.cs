namespace EnGee.Models
{
    public class Rong_CCollectWrap
    {
        private TCollect _coll;
        public TCollect coll
        {
            get { return _coll; }
            set { _coll = value; }
        }
        public Rong_CCollectWrap()
        {
            _coll = new TCollect();
        }
        public int CollectId
        {
            get { return _coll.CollectId; }
            set { _coll.CollectId = value; }
        }
        public int MemberId
        {
            get { return _coll.MemberId; }
            set { _coll.MemberId = value; }
        }
        public string CollectTitle
        {
            get { return _coll.CollectTitle; }
            set { _coll.CollectTitle = value; }
        }
        public string CollectCaption
        {
            get { return _coll.CollectCaption; }
            set { _coll.CollectCaption = value; }
        }
        public DateTime CollectEndDate
        {
            get { return _coll.CollectEndDate; }
            set { _coll.CollectEndDate = value; }
        }
        public int DeliveryTypeId
        {
            get { return _coll.DeliveryTypeId; }
            set { _coll.DeliveryTypeId = value; }
        }
        public string DeliveryAddress
        {
            get { return _coll.DeliveryAddress; }
            set { _coll.DeliveryAddress = value; }
        }
        public string ConvenienNum
        {
            get { return _coll.ConvenienNum; }
            set { _coll.ConvenienNum = value; }
        }

        public bool CollectStatus
        {
            get { return _coll.CollectStatus; }
            set { _coll.CollectStatus = value; }
        }

        public string? CollectImagePath
        {
            get { return _coll.CollectImagePath; }
            set { _coll.CollectImagePath = value; }
        }

        public IFormFile photo { get; set; }

        public string CollectItemName
        {
            get { return _coll.CollectItemName; }
            set { _coll.CollectItemName = value; }
        }
        public int MainCategoryId
        {
            get { return _coll.MainCategoryId; }
            set { _coll.MainCategoryId = value; }
        }

        public int SubcategoryId
        {
            get { return _coll.SubcategoryId; }
            set { _coll.SubcategoryId = value; }
        }
        public int CollectAmount
        {
            get { return _coll.CollectAmount; }
            set { _coll.CollectAmount = value; }
        }


    }
}
