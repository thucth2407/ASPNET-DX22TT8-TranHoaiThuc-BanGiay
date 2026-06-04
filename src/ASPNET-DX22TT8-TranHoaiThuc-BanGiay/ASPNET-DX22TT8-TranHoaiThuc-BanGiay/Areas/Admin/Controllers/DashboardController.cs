using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Models;

namespace ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            var name = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }
    }
}
