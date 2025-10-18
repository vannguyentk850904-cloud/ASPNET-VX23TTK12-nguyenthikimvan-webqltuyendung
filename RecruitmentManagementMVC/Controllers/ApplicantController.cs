using System.Web.Mvc;
using RecruitmentManagementMVC.Filters;

namespace RecruitmentManagementMVC.Controllers
{
    [AuthorizeRole("User", "Admin")]
    public class ApplicantController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Trang dành cho ứng viên đã đăng nhập";
            return View();
        }
    }
}
