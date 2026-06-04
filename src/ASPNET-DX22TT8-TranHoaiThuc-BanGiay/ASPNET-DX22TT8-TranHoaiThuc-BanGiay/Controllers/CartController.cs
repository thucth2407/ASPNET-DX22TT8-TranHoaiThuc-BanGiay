using ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Models;
using Microsoft.AspNetCore.Mvc;
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
    }
}