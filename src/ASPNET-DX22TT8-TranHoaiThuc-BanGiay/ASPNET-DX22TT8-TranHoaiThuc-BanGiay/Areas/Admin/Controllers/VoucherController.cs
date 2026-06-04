using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Models;
using ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Repository;

namespace ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/ma-giam-gia/[action]/{id?}")]
    public class VoucherController : Controller
    {
        private readonly DataContext _dataContext;
        public VoucherController(DataContext context)
        {
            _dataContext = context;
        }
        [Route("/admin/ma-giam-gia")]
        [Route("/admin/ma-giam-gia/index")]
        public IActionResult Index()
        {
            var name = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Index", "Login");
            }
            var vouchers = _dataContext.Voucher.ToList();
            return View(vouchers);
        }
        [Route("/admin/ma-giam-gia/tao-moi")]
        public IActionResult Create()
        {
            var name = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }

        [HttpPost]
        [Route("/admin/ma-giam-gia/tao-moi")]
        public async Task<IActionResult> Create(VoucherModel voucher)
        {
            var name = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Index", "Login");
            }
            if (voucher.Code == null)
            {
                return View(voucher);
            }
            //Lưu vào DB
            _dataContext.Add(voucher);
            await _dataContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [Route("/admin/ma-giam-gia/cap-nhat/{id?}")]
        public async Task<IActionResult> Edit(int id)
        {
            var name = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Index", "Login");
            }
            var voucher = await _dataContext.Voucher.FindAsync(id);
            return View(voucher);
        }

        [HttpPost]
        [Route("/admin/ma-giam-gia/cap-nhat/{id?}")]
        public async Task<IActionResult> Edit(VoucherModel voucher)
        {
            var name = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Index", "Login");
            }
            _dataContext.Update(voucher);
            await _dataContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int Id)
        {
            VoucherModel voucher = await _dataContext.Voucher.FindAsync(Id);

            _dataContext.Voucher.Remove(voucher);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
