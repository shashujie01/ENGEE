namespace EnGee.ViewModels
{
    public class Request_Rootobject
    {
        public string returnCode { get; set; }
        public string returnMessage { get; set; }
        public Request_Info info { get; set; }
    }

    public class Request_Info
    {
        public long transactionId { get; set; }
        public Request_PaymentUrl paymentUrl { get; set; }
    }

    public class Request_PaymentUrl
    {
        public string web { get; set; }
    }
    public class ConfirmResponse
    {
        public string returnCode { get; set; }
        public string returnMessage { get; set; }
        // 其他可能的屬性...
    }
}
