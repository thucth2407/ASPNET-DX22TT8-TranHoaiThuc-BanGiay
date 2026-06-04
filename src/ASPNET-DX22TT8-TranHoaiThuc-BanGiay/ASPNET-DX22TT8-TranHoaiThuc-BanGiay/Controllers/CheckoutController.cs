using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Extensions;
using ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Models;
using ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Repository;
using ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Services.Vnpay;

namespace ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DataContext _dataContext;
        private static Random random = new Random();        
        private readonly IVnPayService _vnPayService;
        public CheckoutController(DataContext context, IVnPayService vnPayService)
        {
            _dataContext = context;            
            _vnPayService = vnPayService;
        }
        [Route("Checkout")]
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartModel>>("cart");

            if (cart == null || !cart.Any())
                return RedirectToAction("Index", "Cart");

            decimal subTotal = cart.Sum(i => i.Price * i.Quantity);
            decimal discountAmt = HttpContext.Session.GetDecimal("CouponDiscountAmount");   // đọc từ Session
            string? appliedCode = HttpContext.Session.GetString("AppliedCouponCode") ?? "";
            decimal total = Math.Max(0, subTotal - discountAmt);

            ViewData["SubTotal"] = subTotal.ToString("#,0");
            ViewData["DiscountAmount"] = discountAmt;    // decimal — View tự format
            ViewData["AppliedCode"] = appliedCode;
            ViewData["Total"] = total.ToString("#,0");
            ViewData["CountItemCart"] = cart.Count;

            return View();
        }
        //Status: 1: Chưa thanh toán | 2: đã thanh toán | 3: lỗi
        [HttpPost]
        [Route("Checkout")]
        public async Task<IActionResult> Index(
    string name, string address, string email, string phone,
    decimal subtotal, decimal discount, decimal total,
    string paymentmethod)
        {
            HttpContext.Session.SetDecimal("CustomerTotal", total);
            HttpContext.Session.SetDecimal("CustomerSubTotal", subtotal);

            var order = new OrderModel
            {
                OrderId = DateTime.Now.ToString("ddMMyy") + RandomString(6),
                Name = name,
                Address = address,
                Email = email,
                Phone = phone,
                PaymentMethod = Convert.ToInt32(paymentmethod),
                Discount = discount,
                Status = 1,
                Total = total,
                Created_at = DateTime.Now
            };

            _dataContext.Orders.Add(order);
            await _dataContext.SaveChangesAsync();

            var cart = HttpContext.Session.GetObjectFromJson<List<CartModel>>("cart");
            if (cart == null || !cart.Any())
                throw new ArgumentException("Giỏ hàng không có sản phẩm!");

            var orderDetails = cart.Select(item => new OrderDetailModel
            {
                OrderId = order.Id,
                ProductId = item.Id,
                Quantity = item.Quantity,
                Total = item.Quantity * item.Price
            }).ToList();

            _dataContext.OrderDetails.AddRange(orderDetails);
            await _dataContext.SaveChangesAsync();

            // Xoá giỏ hàng + coupon khỏi Session
            HttpContext.Session.Remove("cart");
            HttpContext.Session.Remove("AppliedCouponCode");
            HttpContext.Session.Remove("CouponDiscountAmount");

            return paymentmethod switch
            {
                "1" => RedirectToAction("Index", "Approve"),           // COD
                "2" => Redirect(_vnPayService.CreatePaymentUrl(order, HttpContext)), // VNPay
                _ => View()
            };
        }
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}