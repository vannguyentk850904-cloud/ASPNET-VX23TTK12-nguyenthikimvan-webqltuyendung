using System.Linq;
using System.Web.Mvc;
using RecruitmentManagementMVC.Models; // nhớ thêm dòng này

public class HomeController : Controller
{
    private RecruitmentDbEntities db = new RecruitmentDbEntities();

    public ActionResult Index()
    {
        var latestJobs = db.JobPosts
            .OrderByDescending(j => j.CreatedAt)
            .Take(6)
            .ToList();

        return View(latestJobs);
    }

    public ActionResult About()
    {
        return View();
    }

    public ActionResult Contact()
    {
        return View();
    }
}
