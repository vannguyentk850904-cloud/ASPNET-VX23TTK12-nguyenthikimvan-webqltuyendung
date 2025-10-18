using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using RecruitmentManagementMVC.Models;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace RecruitmentManagementMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AccountController()
        {
            var context = new ApplicationDbContext();
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
        }

        // ======================== ĐĂNG KÝ ========================
        [HttpGet]
        public ActionResult Register()
        {
            ViewBag.Roles = new SelectList(new[] { "User", "Recruiter", "Admin" });
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(string fullName, string email, string password, string role)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName
            };

            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                // Nếu role chưa có thì tạo
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }

                // Gán quyền cho user mới
                await userManager.AddToRoleAsync(user.Id, role);

                TempData["Success"] = "Đăng ký thành công! Mời đăng nhập.";
                return RedirectToAction("Login");
            }

            ViewBag.Error = string.Join(", ", result.Errors);
            ViewBag.Roles = new SelectList(new[] { "User", "Recruiter", "Admin" });
            return View();
        }

        // ======================== ĐĂNG NHẬP ========================
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(string email, string password)
        {
            var user = await userManager.FindAsync(email, password);

            if (user != null)
            {
                var authManager = HttpContext.GetOwinContext().Authentication;
                var identity = await user.GenerateUserIdentityAsync(userManager);

                authManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                authManager.SignIn(new AuthenticationProperties { IsPersistent = true }, identity);

                // ✅ Điều hướng theo role
                if (await userManager.IsInRoleAsync(user.Id, "Admin"))
                    return RedirectToAction("Index", "Admin");

                if (await userManager.IsInRoleAsync(user.Id, "Recruiter"))
                    return RedirectToAction("Dashboard", "Recruiter");

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Sai email hoặc mật khẩu!";
            return View();
        }

        // ======================== ĐĂNG XUẤT ========================
        public ActionResult Logout()
        {
            var authManager = HttpContext.GetOwinContext().Authentication;
            authManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        // ======================== TẠO ROLE MẶC ĐỊNH ========================
        // Gọi hàm này 1 lần ở Startup.Auth hoặc Global.asax
        public void CreateDefaultRoles()
        {
            var roles = new[] { "Admin", "Recruiter", "User" };
            foreach (var roleName in roles)
            {
                if (!roleManager.RoleExists(roleName))
                    roleManager.Create(new IdentityRole(roleName));
            }
        }
    }
}
