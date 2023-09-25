using System.Configuration;

namespace EnGee.Models
{
    public class Rong_CDonationWrap
    {
        private TDonationOrder _don;
        public TDonationOrder don
        {
            get { return _don; }
            set { _don = value; }
        }
       
        public Rong_CDonationWrap()
        {
            _don = new TDonationOrder();
        }
        public int DonationOrderId
        {
            get { return _don.DonationOrderId; }
            set { _don.DonationOrderId = value; }
        }
        //public int CollectId
        //{
        //    get { return _don.CollectId; }
        //    set { _don.CollectId = value; }
        //}
        public string CollectItemName { get; set; }

        public int CollectAmount { get; set; }
        //public int MemberId
        //{
        //    get { return _don.MemberId; }
        //    set { _don.MemberId = value; }
        //}
        //public DateTime OrderDate
        //{
        //    get { return _don.OrderDate; }
        //    set { _don.OrderDate = value; }
        //}
        public string DonarName
        {
            get { return _don.DonarName; }
            set { _don.DonarName = value; }
        }
        public string DonarPhone
        {
            get { return _don.DonarPhone; }
            set { _don.DonarPhone = value; }
        }

        public int DonationAmount
        {
            get { return _don.DonationAmount; }
            set { _don.DonationAmount = value; }
        }
        public string DonationStatus
        {
            get { return _don.DonationStatus; }
            set { _don.DonationStatus = value; }
        }
    }
}
