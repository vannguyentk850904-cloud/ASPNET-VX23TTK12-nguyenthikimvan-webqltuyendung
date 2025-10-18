using System.Web.Mvc;
using RecruitmentManagementMVC.Filters;

namespace RecruitmentManagementMVC.Controllers
{
    [AuthorizeRole("Recruiter", "Admin")]
    public class RecruiterController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Trang dành cho nhà tuyển dụng";
            return View();
        }
    }
}
