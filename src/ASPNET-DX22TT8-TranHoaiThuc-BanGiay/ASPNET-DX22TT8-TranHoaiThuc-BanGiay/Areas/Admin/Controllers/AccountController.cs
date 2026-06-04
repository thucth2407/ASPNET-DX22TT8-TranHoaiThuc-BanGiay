using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Models;
using ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Repository;
using System.Text.RegularExpressions;

namespace ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/tai-khoan/[action]/{id?}")]
    public class AccountController : Controller
    {
        private readonly DataContext _dataContext;
        public AccountController(DataContext context)
        {
            _dataContext = context;
        }
        [Route("/admin/tai-khoan")]
        [Route("/admin/tai-khoan/index")]
        public IActionResult Index()
        {
            var name = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Index", "Login");
            }
            var users = _dataContext.Users.ToList();
            return View(users);
        }
        [Route("/admin/tai-khoan/tao-moi")]
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
        public async Task<IActionResult> Create(UserModel Users)
        {
            var checkEmail = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == Users.Email);            
            if (checkEmail != null)
            {
                return View();
            }

            string BCryptPassword = BCrypt.Net.BCrypt.HashPassword(Users.Password);
            //Lưu vào DB
            Users.Created_at = DateTime.Now;
            Users.Updated_at = DateTime.Now;            
            Users.Password = BCryptPassword;
            _dataContext.Add(Users);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Route("/admin/tai-khoan/cap-nhat/{id?}")]
        public async Task<IActionResult> Edit(int id)
        {
            var name = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Index", "Login");
            }
            var Users = await _dataContext.Users.FindAsync(id);
            if (Users == null)
            {
                return NotFound();
            }
            return View(Users);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UserModel Users)
        {
            var checkEmail = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == Users.Email);
            var checkPhone = await _dataContext.Users.FirstOrDefaultAsync(u => u.Phone == Users.Phone);
            var user = await _dataContext.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }            
            if (checkEmail != null)
            {
                if (checkEmail.Id != id)
                {
                    return View(Users);
                }
            }            
            if (checkPhone != null)
            {
                if (checkPhone.Id != id)
                {
                    return View(Users);
                }
            }            
            if (!string.IsNullOrEmpty(Users.Password) || !string.IsNullOrEmpty(Users.ConfirmPassword))
            {
                var hasNumber = new Regex(@"[0-9]+");
                var hasUpperChar = new Regex(@"[A-Z]+");
                var hasMiniMaxChars = new Regex(@".{6,15}");
                var hasLowerChar = new Regex(@"[a-z]+");
                var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");
                if (!hasLowerChar.IsMatch(Users.Password) || !hasLowerChar.IsMatch(Users.Password) || !hasLowerChar.IsMatch(Users.Password) || !hasLowerChar.IsMatch(Users.Password) || !hasLowerChar.IsMatch(Users.Password))
                {
                    return View(Users);
                }
                if (Users.Password.Contains(" "))
                {
                    return View(Users);
                }
                if (Users.Password != Users.ConfirmPassword)
                {
                    return View(Users);
                }                
                bool verifiPassword = BCrypt.Net.BCrypt.Verify(Users.Password, user.Password);
                if (verifiPassword == true)
                {
                    return View(Users);
                }
                string BCryptPassword = BCrypt.Net.BCrypt.HashPassword(Users.Password);
                user.Password = BCryptPassword;
            }
            else
            {
                user.Password = user.Password;
            }

            //Lưu vào DB
            if (checkPhone == null)
            {
                user.Phone = Users.Phone;
            }
            if (checkEmail == null)
            {
                user.Email = Users.Email;
            }
            user.Name = Users.Name;
            user.Actived = Users.Actived;
            user.Updated_at = DateTime.Now;
            await _dataContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int Id)
        {
            UserModel user = await _dataContext.Users.FindAsync(Id);

            _dataContext.Users.Remove(user);
            await _dataContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
