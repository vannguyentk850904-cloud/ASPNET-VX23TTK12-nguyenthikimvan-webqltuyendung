using RecruitmentManagementMVC.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

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
                // Kiểm tra email đã tồn tại
                var existingUser = db.Users.FirstOrDefault(u => u.Email == user.Email);
                if (existingUser != null)
                {
                    ViewBag.Error = "Email đã được sử dụng!";
                    return View(user);
                }

                // Ngăn người dùng tự đăng ký quyền Admin
                if (user.Role == "Admin")
                {
                    ViewBag.Error = "Bạn không thể đăng ký tài khoản quản trị!";
                    return View(user);
                }

                user.CreatedAt = DateTime.Now;

                // Nếu không chọn vai trò thì mặc định là Ứng viên
                if (string.IsNullOrEmpty(user.Role))
                {
                    user.Role = "Candidate";
                }

                // Gán trạng thái duyệt
                if (user.Role == "Employer")
                {
                    user.IsApproved = false; // Nhà tuyển dụng cần được admin duyệt
                }
                else
                {
                    user.IsApproved = true; // Ứng viên được kích hoạt ngay
                }

                db.Users.Add(user);
                db.SaveChanges();

                if (user.Role == "Employer")
                {
                    TempData["Success"] = "Đăng ký thành công! Vui lòng chờ quản trị viên duyệt tài khoản của bạn.";
                }
                else
                {
                    TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                }

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

            var user = db.Users.FirstOrDefault(u => u.Email == email && u.PasswordHash == password);
            if (user == null)
            {
                ViewBag.Error = "Sai email hoặc mật khẩu!";
                return View();
            }

            // ❌ Nếu là Employer mà chưa được duyệt
            if (user.Role == "Employer" && !user.IsApproved)
            {
                ViewBag.Error = "Tài khoản của bạn chưa được quản trị viên duyệt. Vui lòng quay lại sau!";
                return View();
            }

            // ✅ Lưu cookie xác thực
            FormsAuthentication.SetAuthCookie(user.Email, false);

            // ✅ Lưu thông tin Session
            Session["UserId"] = user.UserId;
            Session["FullName"] = user.FullName;
            Session["Role"] = user.Role;

            // ✅ Điều hướng theo vai trò
            switch (user.Role)
            {
                case "Admin":
                    return RedirectToAction("Index", "Admin");

                case "Employer":
                    return RedirectToAction("Index", "JobPosts");

                case "Candidate":
                default:
                    return RedirectToAction("Index", "Home");
            }
        }




        // ========== ĐĂNG XUẤT ==========
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut(); // ✅ xoá cookie xác thực
            Session.Clear();
            return RedirectToAction("Login");
        }

    }
}
