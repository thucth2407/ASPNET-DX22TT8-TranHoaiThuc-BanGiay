using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Extensions;
using ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Models;
using ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Repository;

namespace ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Controllers
{
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        public ProductController(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IActionResult> Index()
        {
            string category = HttpContext.Request.Query["category"].ToString();
            string search = HttpContext.Request.Query["search"].ToString();
            string sort = HttpContext.Request.Query["sort"].ToString();

            var productsQuery = _dataContext.Products
                .Include(c => c.Category)                
                .Where(p => p.Status != 0);

            // Lọc theo category slug
            if (!string.IsNullOrEmpty(category))
            {
                CategoryModel categoryModel = _dataContext.Categories
                    .Where(c => c.Slug == category).FirstOrDefault();
                if (categoryModel == null) return RedirectToAction("Index");
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryModel.Id);
            }

            // Lọc theo search
            if (!string.IsNullOrEmpty(search))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(search));
            }

            // Sắp xếp
            productsQuery = sort switch
            {
                "price_asc" => productsQuery.OrderBy(p => p.Price),
                "price_desc" => productsQuery.OrderByDescending(p => p.Price),
                "newest" => productsQuery.OrderByDescending(p => p.Created_at),
                _ => productsQuery.OrderByDescending(p => p.Created_at)
            };

            // Cart count
            var cart = HttpContext.Session.GetObjectFromJson<List<CartModel>>("cart");
            int countItem = cart?.Count ?? 0;
            ViewData["CountItemCart"] = countItem;

            // Truyền categories và filter hiện tại xuống view
            ViewBag.Categories = _dataContext.Categories
                .Where(c => c.Status == 1)
                .Select(c => new {
                    c.Id,
                    c.Name,
                    c.Slug,
                    Count = _dataContext.Products.Count(p => p.CategoryId == c.Id && p.Status != 0)
                }).ToList();
            ViewBag.products = _dataContext.Products
                .Include(c => c.Category)                
                .Where(p => p.Status != 0)
                .ToList();
            ViewBag.CurrentCategory = category;
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentSort = sort;

            return View(await productsQuery.ToListAsync());
        }

        public IActionResult Detail(int? Id)
        {
            if (Id == null) return RedirectToAction("Index");

            var product = _dataContext.Products
                .Include(p => p.Category)                
                .Where(p => p.Id == Id)
                .FirstOrDefault();

            if (product == null) return RedirectToAction("Index");

            //var sizes = _dataContext.ProductSize
            //    .Where(p => p.ProductId == Id)
            //    .ToList();

            // Sản phẩm liên quan cùng danh mục
            var related = _dataContext.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == product.CategoryId && p.Id != Id && p.Status != 0)
                .Take(4)
                .ToList();

            var cart = HttpContext.Session.GetObjectFromJson<List<CartModel>>("cart");
            ViewData["CountItemCart"] = cart?.Count ?? 0;

            //ViewBag.Size = sizes;
            ViewBag.Related = related;
            return View(product);
        }
    }
}