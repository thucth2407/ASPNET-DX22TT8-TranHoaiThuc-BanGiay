using ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Extensions;
using ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Repository;

namespace ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Controllers
{
    public class CartController : Controller
    {
        private readonly DataContext _dataContext;
        public CartController(DataContext context)
        {
            _dataContext = context;
        }
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartModel>>("cart");
            Decimal subtotal = 0;
            Decimal total = 0;
            int countItem = 0;
            if (cart == null)
            {
                cart = new List<CartModel>();
            }
            else
            {
                //Tính tổng giá tiền giỏ hàng
                foreach (var item in cart)
                {
                    subtotal += item.Price * item.Quantity;
                    total += item.Price * item.Quantity;
                    countItem++;
                }
            }
            HttpContext.Session.SetInt32("subtotal", Convert.ToInt32(subtotal));
            HttpContext.Session.SetInt32("total", Convert.ToInt32(total));

            //HttpContext.Session.SetString("Userid", user.Id.ToString());
            ViewData["SubTotal"] = subtotal.ToString("#,0");
            ViewData["Total"] = total.ToString("#,0");
            ViewData["CountItemCart"] = countItem;
            return View(cart);
        }

        public IActionResult AddToCart(int Id, string Name, decimal Price, int quantity, int Type, string imageUrl, string size = "")
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartModel>>("cart");
            //Nếu giỏ hàng trống -> thêm sản phẩm vào giỏ hàng
            if (cart == null)
            {
                cart = new List<CartModel>();
            }

            // Check trùng: cùng Id VÀ cùng Size mới cộng dồn
            var item = cart.FirstOrDefault(c => c.Id == Id && c.Size == size);
            if (item == null)
            {
                cart.Add(new CartModel
                {
                    Id = Id,
                    Name = Name,
                    Price = Price,
                    Quantity = quantity,
                    ImageUrl = imageUrl,
                    Size = size
                });
            }
            else
            {
                item.Quantity += quantity;
            }
            HttpContext.Session.SetObjectAsJson("cart", cart);
            if (Type == 1)
            {
                return Json(true);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId, string action)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartModel>>("cart");
            if (cart == null) return RedirectToAction("Index");

            var item = cart.FirstOrDefault(c => c.Id == productId);
            if (item != null)
            {
                if (action == "increase")
                    item.Quantity++;
                else if (action == "decrease" && item.Quantity > 1)
                    item.Quantity--;
            }

            HttpContext.Session.SetObjectAsJson("cart", cart);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RemoveCart(int id)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartModel>>("cart");
            if (cart != null)
            {
                var item = cart.FirstOrDefault(c => c.Id == id);
                if (item != null) cart.Remove(item);
                HttpContext.Session.SetObjectAsJson("cart", cart);
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyCoupon([FromBody] ApplyCouponRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Code))
                return Json(new { success = false, message = "Vui lòng nhập mã giảm giá." });

            var code = request.Code.Trim().ToUpper();

            // Kiểm tra mã trong DB
            var voucher = await _dataContext.Voucher
                .FirstOrDefaultAsync(v => v.Code != null &&
                                          v.Code.ToUpper() == code &&
                                          v.Status == 1 &&          // 1 = còn hiệu lực
                                          v.Date >= DateTime.Now);  // chưa hết hạn

            if (voucher == null)
                return Json(new { success = false, message = "Mã giảm giá không hợp lệ hoặc đã hết hạn." });

            // Lấy giỏ hàng để tính số tiền giảm
            var cart = HttpContext.Session.GetObjectFromJson<List<CartModel>>("cart") ?? new List<CartModel>();
            if (!cart.Any())
                return Json(new { success = false, message = "Giỏ hàng đang trống." });

            decimal subTotal = cart.Sum(i => i.Price * i.Quantity);

            decimal discountAmount = Math.Min(voucher.DiscountAmount ?? 0, subTotal);

            // Lưu vào Session để Checkout đọc lại
            HttpContext.Session.SetString("AppliedCouponCode", voucher.Code!);
            HttpContext.Session.SetDecimal("CouponDiscountAmount", discountAmount);

            return Json(new
            {
                success = true,
                code = voucher.Code,
                discountAmount = discountAmount,
                message = $"Áp dụng mã {voucher.Code} thành công!"
            });
        }

        // ── 2. RemoveCoupon ─────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveCoupon()
        {
            HttpContext.Session.Remove("AppliedCouponCode");
            HttpContext.Session.Remove("CouponDiscountAmount");
            return Json(new { success = true });
        }

        // ── DTO cho ApplyCoupon request body ────────────────────────
        public class ApplyCouponRequest
        {
            public string? Code { get; set; }
        }
    }
}