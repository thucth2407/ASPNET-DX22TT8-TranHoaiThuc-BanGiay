namespace ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Models.Vnpay
{
    public class PaymentInformationModel
    {
        public string OrderType { get; set; }
        public double Amount { get; set; }
        public string OrderDescription { get; set; }
        public string Name { get; set; }

        public string OrderId { get; set; }
    }
}
