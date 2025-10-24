using System;
using System.Web;
using System.Web.Mvc;

namespace RecruitmentManagementMVC.Filters
{
    /// <summary>
    /// Bộ lọc phân quyền dựa trên Session["Role"]
    /// Dùng: [AuthorizeRole("Admin", "Recruiter")]
    /// </summary>
    public class AuthorizeRoleAttribute : AuthorizeAttribute
    {
        private readonly string[] allowedRoles;

        public AuthorizeRoleAttribute(params string[] roles)
        {
            this.allowedRoles = roles;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var userRole = httpContext.Session["Role"] as string;

            // Chưa đăng nhập
            if (string.IsNullOrEmpty(userRole))
                return false;

            // Nếu là admin thì luôn được phép truy cập
            if (userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                return true;

            // Nếu không phải admin → kiểm tra có trong danh sách roles không
            foreach (var role in allowedRoles)
            {
                if (string.Equals(userRole, role, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var userRole = filterContext.HttpContext.Session["Role"] as string;

            if (string.IsNullOrEmpty(userRole))
            {
                // Nếu chưa đăng nhập → về trang đăng nhập
                filterContext.Result = new RedirectResult("/Account/Login");
            }
            else
            {
                // Nếu đăng nhập rồi nhưng không đủ quyền → về trang chủ
                filterContext.Result = new RedirectResult("/Home/Index");
            }
        }
    }
}
