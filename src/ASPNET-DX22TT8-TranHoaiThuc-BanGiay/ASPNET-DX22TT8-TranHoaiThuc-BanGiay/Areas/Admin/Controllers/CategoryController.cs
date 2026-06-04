using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Models;
using ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Repository;

namespace ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Areas.Admin.Controllers
{
    [Area("Admin")]    
    [Route("admin/danh-muc/[action]/{id?}")]
    public class CategoryController : Controller
    {
        private readonly DataContext _dataContext;
        public CategoryController(DataContext context)
        {
            _dataContext = context;
        }
        [Route("/admin/danh-muc")]
        [Route("/admin/danh-muc/index")]
        public IActionResult Index()
        {
            var name = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Index", "Login");
            }
            var categories = _dataContext.Categories.ToList();
            return View(categories);
        }        
        [Route("/admin/danh-muc/tao-moi")]
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
        public async Task<IActionResult> Create(CategoryModel category)
        {
            var name = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Index", "Login");
            }
            category.Slug = category.Name.Replace(" ", "-");
            var slug = await _dataContext.Categories.FirstOrDefaultAsync(p => p.Slug == category.Slug);
            if (slug != null)
            {
                return View(category);
            }
            if (category.Name == null)
            {
                return View(category);
            }
            //Lưu vào DB
            _dataContext.Add(category);
            await _dataContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [Route("/admin/danh-muc/cap-nhat/{id?}")]
        public async Task<IActionResult> Edit(int id)
        {
            var name = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Index", "Login");
            }
            var category = await _dataContext.Categories.FindAsync(id);
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryModel category)
        {
            var name = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Index", "Login");
            }
            category.Slug = category.Name.Replace(" ", "-");

            _dataContext.Update(category);
            await _dataContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int Id)
        {
            CategoryModel category = await _dataContext.Categories.FindAsync(Id);

            _dataContext.Categories.Remove(category);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
