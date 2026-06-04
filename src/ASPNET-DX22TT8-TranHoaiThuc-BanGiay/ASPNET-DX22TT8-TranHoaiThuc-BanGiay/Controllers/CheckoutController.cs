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

            // Nếu giỏ hàng trống thì redirect về cart
            if (cart == null || !cart.Any())
                return RedirectToAction("Index", "Cart");

            Decimal subtotal = cart.Sum(i => i.Price * i.Quantity);
            Decimal total = subtotal;
            int countItem = cart.Count;

            ViewData["SubTotal"] = subtotal.ToString("#,0");
            ViewData["Total"] = total.ToString("#,0");
            ViewData["CountItemCart"] = countItem;
            return View();
        }
        //Status: 1: Chưa thanh toán | 2: đã thanh toán | 3: lỗi
        [HttpPost]
        [Route("Checkout")]
        public async Task<IActionResult> Index(string name, string address, string email, string phone, decimal subtotal, decimal discount, decimal total, string paymentmethod)
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
                Status = 1,
                Total = total,
                Created_at = DateTime.Now
            };

            _dataContext.Orders.Add(order);
            await _dataContext.SaveChangesAsync();

            var cart = HttpContext.Session.GetObjectFromJson<List<CartModel>>("cart");
            if (cart == null || !cart.Any())
            {
                throw new ArgumentException("Giỏ hàng không có sản phẩm!");
            }
            else
            {
                foreach (var item in cart)
                {
                    // Tìm đúng size của sản phẩm trong cart
                    //var productSize = await _dataContext.ProductSize
                    //    .Where(ps => ps.ProductId == item.Id && ps.Size == item.Size)
                    //    .FirstOrDefaultAsync();

                    //if (productSize != null)
                    //{
                    //    if (productSize.Quantity >= item.Quantity)
                    //    {
                    //        // Trừ tồn kho theo size
                    //        productSize.Quantity -= item.Quantity;
                    //        _dataContext.ProductSize.Update(productSize);
                    //        await _dataContext.SaveChangesAsync();
                    //    }
                    //    else
                    //    {
                    //        // Không đủ hàng - xóa order vừa tạo và về cart
                    //        _dataContext.Orders.Remove(order);
                    //        await _dataContext.SaveChangesAsync();
                    //        TempData["ErrorMessage"] = $"Sản phẩm '{item.Name}' size {item.Size} không đủ số lượng!";
                    //        return RedirectToAction("Index", "Cart");
                    //    }
                    //}
                    _dataContext.Orders.Remove(order);
                    await _dataContext.SaveChangesAsync();
                    TempData["ErrorMessage"] = $"Sản phẩm '{item.Name}' size {item.Size} không đủ số lượng!";
                    // Nếu không có size (sản phẩm không quản lý size) thì bỏ qua check
                }
            }

            var orderDetails = cart.Select(item => new OrderDetailModel
            {
                OrderId = order.Id,
                ProductId = item.Id,
                Quantity = item.Quantity,
                Total = item.Quantity * item.Price
            }).ToList();

            _dataContext.OrderDetails.AddRange(orderDetails);
            _dataContext.SaveChanges();
            HttpContext.Session.Remove("cart");
            switch (paymentmethod)
            {
                case "1": //Thanh toán tại cửa hàng    
                    return RedirectToAction("Index", "Approve");
                case "2": //VNPay                                 
                    var url = _vnPayService.CreatePaymentUrl(order, HttpContext);
                    return Redirect(url);
                default:
                    return View();
            }
        }
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}