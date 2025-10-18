using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using RecruitmentManagementMVC.Models;
using System;

[assembly: OwinStartup(typeof(RecruitmentManagementMVC.Startup))]
namespace RecruitmentManagementMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Cấu hình xác thực (cookie, user manager, role manager)
            ConfigureAuth(app);

            // ✅ Tạo các role & admin mặc định
            CreateDefaultRolesAndAdmin();
        }

        // ======================== CẤU HÌNH XÁC THỰC ========================
        public void ConfigureAuth(IAppBuilder app)
        {
            // Mỗi request sẽ có instance riêng của ApplicationDbContext, UserManager, RoleManager
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);

            // Cấu hình cookie đăng nhập
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                LogoutPath = new PathString("/Account/Logout"),
                ExpireTimeSpan = TimeSpan.FromHours(2),
                SlidingExpiration = true
            });
        }

        // ======================== TẠO ROLE & ADMIN MẶC ĐỊNH ========================
        private void CreateDefaultRolesAndAdmin()
        {
            // ✅ Dùng ApplicationDbContext (Identity context), KHÔNG dùng RecruitmentEntities
            using (var context = new ApplicationDbContext())
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                // Danh sách role mặc định
                string[] roles = { "Admin", "Recruiter", "User" };

                foreach (var role in roles)
                {
                    if (!roleManager.RoleExists(role))
                    {
                        roleManager.Create(new IdentityRole(role));
                    }
                }

                // ✅ Tạo tài khoản Admin mặc định (nếu chưa có)
                var adminEmail = "admin@jobportal.com";
                var adminUser = userManager.FindByEmail(adminEmail);

                if (adminUser == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        FullName = "Administrator"
                    };

                    var result = userManager.Create(user, "Admin@123"); // Mật khẩu mặc định
                    if (result.Succeeded)
                    {
                        userManager.AddToRole(user.Id, "Admin");
                    }
                }
            }
        }
    }

    // ======================== USER MANAGER ========================
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            return manager;
        }
    }

    // ======================== ROLE MANAGER ========================
    public class ApplicationRoleManager : RoleManager<IdentityRole>
    {
        public ApplicationRoleManager(IRoleStore<IdentityRole, string> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(new RoleStore<IdentityRole>(context.Get<ApplicationDbContext>()));
        }
    }
}
