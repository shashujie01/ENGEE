namespace EnGee.Models
{
    public class CHI_CMemberWrap : TMember
    {

        private TMember _member;
        public CHI_CMemberWrap()
        {
            _member = new TMember();
        }

        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }

        public string Username
        {
            get { return _member.Username; }
            set { _member.Username = value; }
        }
        public string Introduction
        {
            get { return _member.Introduction; }
            set { _member.Introduction = value; }
        }

        public TMember member
        {
            get { return _member; }
            set { _member = value; }
        }
        public int MemberId
        {
            get { return _member.MemberId; }
            set { _member.MemberId = value; }
        }
        public string? PhotoPath
        {
            get { return _member.PhotoPath; }
            set { _member.PhotoPath = value; }
        }
        public string Email
        {
            get {return _member.Email; }
            set { _member.Email = value; }
        }

        public DateTime? Birth
        {
            get { return _member.Birth; }
            set { _member.Birth = (DateTime)value; }//20230919合併時加入
        }

        public string Fullname
        {
            get { return _member.Fullname; }
            set { _member.Fullname = value; }
        }

        public string Address
        {
            get { return _member.Address; }
            set { _member.Address = value; }
        }

        public int Gender
        {
            get { return _member.Gender; }
            set { _member.Gender = value; }
        }
        public string Phone
        {
            get { return _member.Phone; }
            set { _member.Phone = value; }
        }

        public string Password
        {
            get { return _member.Password; }
            set { _member.Password = value; }
        }

        public string? CharityProof
        {
            get { return _member.CharityProof; }
            set { _member.CharityProof = value; }
        }

       
        public int? Access
        {
            get { return _member.Access; }
            set { _member.Access = value; }
        }

        public double? Point
        {
            get { return _member.Point; }
            set { _member.Point = value; }
        }


        public IFormFile photo { get; set; }
        public IFormFile photoCharityProof { get; set; }
    }
}
