using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Repository;
using ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Services;
using System;
using System.Diagnostics;
using ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Models;

namespace ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LoginController : Controller
    {
        private readonly DataContext _dataContext;
        private static Random random = new Random();
        private readonly EmailService _emailService;
        public LoginController(DataContext context, EmailService emailService)
        {
            _dataContext = context;
            _emailService = emailService;
        }

        [Route("admin/dang-nhap")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("admin/dang-nhap")]
        public async Task<IActionResult> Index(string username, string password)
        {
            var user = await _dataContext.Users.Where(a => a.Phone == username || a.Email == username).SingleOrDefaultAsync();
            Debug.WriteLine(username);

            if (user != null && user.Actived == 1)
            {
                if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
                {
                    return View();
                }

                HttpContext.Session.SetString("Userid", user.Id.ToString());
                HttpContext.Session.SetString("Username", user.Name);              
                HttpContext.Session.SetString("Useremail", user.Email);
                HttpContext.Session.SetString("Userphone", user.Phone);
              
                _dataContext.Users.Update(user);
                await _dataContext.SaveChangesAsync();

                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        [Route("admin/dang-xuat")]
        public IActionResult LogoutAdmin()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [HttpGet]
        [Route("admin/quen-mat-khau")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [Route("Admin/quen-mat-khau")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user != null)
            {
                var randomString = RandomString(10); //Tạo mật khẩu ngẫu nhiên
                string BCryptPassword = BCrypt.Net.BCrypt.HashPassword(randomString); //Băm mật khẩu

                //Lưu lại mật khẩu mới
                user.Password = BCryptPassword;
                _dataContext.Users.Update(user);
                await _dataContext.SaveChangesAsync();

                await _emailService.SendEmailAsync(email, "Đặt lại mật khẩu", $"Mật khẩu mới của bạn là {randomString}. Vui lòng không tiết lộ mật khẩu của bạn cho bất kì ai để đảm bảo bảo mật!");

                return RedirectToAction("Index", "Login");
            }            
            return View();
        }

        [Route("admin/dang-ky")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Route("admin/dang-ky")]
        public async Task<IActionResult> Register(UserModel Users)
        {
            var checkEmail = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == Users.Email);            
            if (checkEmail != null)
            {
                ModelState.AddModelError("Email", "Email này đã được sử dụng!");
                return View(Users);
            }

            string BCryptPassword = BCrypt.Net.BCrypt.HashPassword(Users.Password);
            //Lưu vào DB
            Users.Created_at = DateTime.Now;
            Users.Updated_at = DateTime.Now;
            Users.Password = BCryptPassword;
            _dataContext.Add(Users);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction("Index", "Login");
        }
    }
}
