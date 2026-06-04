using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Models;
using ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Repository;
using System.Diagnostics;

namespace ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/san-pham/[action]/{id?}")]
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnviroment;
        public ProductController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = context;
            _webHostEnviroment = webHostEnvironment;
        }
        [Route("/admin/san-pham")]
        [Route("/admin/san-pham/index")]
        public IActionResult Index()
        {
            var name = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Index", "Login");
            }
            var products = _dataContext.Products.Include(p => p.Category).ToList();
            return View(products);
        }
        [Route("/admin/san-pham/tao-moi")]
        public IActionResult Create()
        {
            var name = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Index", "Login");
            }

            ViewBag.categories = new SelectList(_dataContext.Categories, "Id", "Name");            

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductModel Product)
        {
            var name = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Index", "Login");
            }

            Product.Slug = Product.Name.Replace(" ", "-");
            ViewBag.categories = new SelectList(_dataContext.Categories, "Id", "Name", Product.CategoryId);

            var slug = await _dataContext.Products.FirstOrDefaultAsync(p => p.Slug == Product.Slug);
            if (slug != null)
            {
                return View(Product);
            }
            if (Product.Name == null)
            {
                return View(Product);
            }
            if (Product.Image != null)
            {
                string uploadsDir = Path.Combine(_webHostEnviroment.WebRootPath, "upload/product");
                //Nếu chưa có thư mục thì tạo thư mục
                if (!Directory.Exists(uploadsDir))
                {
                    Directory.CreateDirectory(uploadsDir);
                }

                string imageName = Guid.NewGuid().ToString() + "_" + Product.Image.FileName;
                string filePath = Path.Combine(uploadsDir, imageName);

                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    await Product.Image.CopyToAsync(fs);
                }

                Product.ImageUrl = imageName;
            }
            //Lưu vào DB
            _dataContext.Add(Product);
            await _dataContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [Route("/admin/san-pham/cap-nhat/{id?}")]
        public async Task<IActionResult> Edit(int id)
        {
            var name = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Index", "Login");
            }

            var category = await _dataContext.Products
            .Include(c => c.Category)            
            .FirstOrDefaultAsync(p => p.Id == id);

            //ViewBag.productsize = category.ProductSizes.ToList();
            ViewBag.categories = new SelectList(_dataContext.Categories, "Id", "Name", category.CategoryId);

            return View(category);            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductModel product)
        {
            var name = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Index", "Login");
            }

            product.Slug = product.Name.Replace(" ", "-");
            ViewBag.categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            var productInDb = await _dataContext.Products.FindAsync(product.Id);

            productInDb.Name = product.Name;
            productInDb.Slug = product.Slug;
            productInDb.Description = product.Description;
            productInDb.Price = product.Price;
            productInDb.Quantity = product.Quantity;
            productInDb.CategoryId = product.CategoryId;
            productInDb.Content = product.Content;
            productInDb.Status = product.Status;


            if (product.Image != null)
            {
                string uploadsDir = Path.Combine(_webHostEnviroment.WebRootPath, "upload/product");
                //Check đã có folder updoad chưa nếu chưa thì tạo folder mới
                if (!Directory.Exists(uploadsDir))
                {
                    Directory.CreateDirectory(uploadsDir);
                }

                ////Xóa hình cũ đi
                if (product.ImageUrl != null)
                {
                    string oldfilePath = Path.Combine(uploadsDir, product.ImageUrl);
                    if (System.IO.File.Exists(oldfilePath))
                    {
                        System.IO.File.Delete(oldfilePath);
                    }
                }

                //Upload hình mới
                string imageName = Guid.NewGuid().ToString() + "_" + product.Image.FileName;
                string filePath = Path.Combine(uploadsDir, imageName);
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    await product.Image.CopyToAsync(fs);
                }

                productInDb.ImageUrl = imageName;
            }
            //_dataContext.Update(product);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? Id)
        {
            var product = await _dataContext.Products.FindAsync(Id);
            if (product.ImageUrl != null)
            {
                string uploadsDir = Path.Combine(_webHostEnviroment.WebRootPath, "upload/product");
                string oldfilePath = Path.Combine(uploadsDir, product.ImageUrl);
                if (System.IO.File.Exists(oldfilePath))
                {
                    System.IO.File.Delete(oldfilePath);
                }
            }
            _dataContext.Products.Remove(product);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }        

        private async Task UpdateOrderTotalAsync(int orderId)
        {
            var order = await _dataContext.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId);
            if (order != null)
            {
                order.Total = await _dataContext.OrderDetails
                    .Where(d => d.OrderId == orderId)
                    .SumAsync(d => d.Total);

                await _dataContext.SaveChangesAsync();
            }
        }
    }
}
