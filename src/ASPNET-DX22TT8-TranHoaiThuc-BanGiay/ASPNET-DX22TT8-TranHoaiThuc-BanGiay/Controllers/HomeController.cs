using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Models;
using Microsoft.EntityFrameworkCore;
using ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Extensions;
using ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Repository;

namespace ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, DataContext context)
        {
            _logger = logger;
            _dataContext = context;
        }

        public IActionResult Index()
        {
            var categories = _dataContext.Categories.ToList();
            var products = _dataContext.Products.Include(p => p.Category).ToList();

            var cart = HttpContext.Session.GetObjectFromJson<List<CartModel>>("cart");
            int countItem = 0;
            if (cart != null)
            {
                foreach (var item in cart)
                {
                    countItem++;
                }
            }
            ViewData["CountItemCart"] = countItem;

            ViewBag.Categories = categories;
            ViewBag.Products = products;
            return View();
        }
    }
}
