using ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Models;
using ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Models.Vnpay;
namespace ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Services.Vnpay
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(OrderModel model, HttpContext context);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
