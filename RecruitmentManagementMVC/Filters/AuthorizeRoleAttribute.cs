using System;
using System.Web;
using System.Web.Mvc;

namespace RecruitmentManagementMVC.Filters
{
    /// <summary>
    /// Attribute kiểm tra quyền người dùng dựa trên Session["Role"]
    /// </summary>
    public class AuthorizeRoleAttribute : AuthorizeAttribute
    {
        private readonly string[] roles;

        // Nhận danh sách role được phép, ví dụ: [AuthorizeRole("Admin", "Recruiter")]
        public AuthorizeRoleAttribute(params string[] roles)
        {
            this.roles = roles;
        }

        // Hàm kiểm tra xem người dùng hiện tại có quyền hợp lệ không
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var userRole = httpContext.Session["Role"] as string;

            // Nếu chưa đăng nhập (chưa có session) → từ chối
            if (string.IsNullOrEmpty(userRole))
                return false;

            // Duyệt danh sách role được phép
            foreach (var role in roles)
            {
                if (string.Equals(role, userRole, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            // Không có role hợp lệ
            return false;
        }

        // Nếu không đủ quyền thì tự động chuyển hướng về trang đăng nhập
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("/Account/Login");
        }
    }
}
