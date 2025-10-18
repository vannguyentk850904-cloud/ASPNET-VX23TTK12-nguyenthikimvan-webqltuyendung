using System;
using System.Linq;
using System.Web.Mvc;
using RecruitmentManagementMVC.Models;

namespace RecruitmentManagementMVC.Controllers
{
    public class AccountController : Controller
    {
        private RecruitmentDbEntities db = new RecruitmentDbEntities();

        // ========== ĐĂNG KÝ ==========
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra email tồn tại
                var existingUser = db.Users.FirstOrDefault(u => u.Email == user.Email);
                if (existingUser != null)
                {
                    ViewBag.Error = "Email đã được sử dụng!";
                    return View(user);
                }

                // Thiết lập thông tin mặc định
                user.CreatedAt = DateTime.Now;
                if (string.IsNullOrEmpty(user.Role))
                    user.Role = "User"; // Mặc định là User

                // Hash mật khẩu (tạm thời lưu plain-text, sau có thể mã hóa)
                db.Users.Add(user);
                db.SaveChanges();

                TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }

            return View(user);
        }

        // ========== ĐĂNG NHẬP ==========
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin!";
                return View();
            }

            // Kiểm tra thông tin đăng nhập
            var user = db.Users.FirstOrDefault(u => u.Email == email && u.PasswordHash == password);
            if (user == null)
            {
                ViewBag.Error = "Sai email hoặc mật khẩu!";
                return View();
            }

            // Lưu thông tin vào Session
            Session["UserId"] = user.UserId;
            Session["FullName"] = user.FullName;
            Session["Role"] = user.Role;

            // Điều hướng theo vai trò
            switch (user.Role)
            {
                case "Admin":
                    return RedirectToAction("Index", "Admin");
                case "Recruiter":
                    return RedirectToAction("Index", "Recruiter");
                default:
                    return RedirectToAction("Index", "Home");
            }
        }

        // ========== ĐĂNG XUẤT ==========
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
