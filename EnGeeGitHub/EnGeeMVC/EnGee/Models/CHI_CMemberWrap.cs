namespace EnGee.Models
{
    public class CHI_CMemberWrap
    {

        private TMember _member;
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

        public string Gender
        {
            get { return _member.Gender; }
            set { _member.Gender = value; }
        }
        public string Phone
        {
            get { return _member.Phone; }
            set { _member.Phone = value; }
        }



        public IFormFile photo { get; set; }
    }
}
