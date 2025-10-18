using System.Web.Mvc;
using RecruitmentManagementMVC.Filters;

namespace RecruitmentManagementMVC.Controllers
{
    [AuthorizeRole("Admin")]
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Trang quản trị hệ thống (Admin)";
            return View();
        }
    }
}
