using System;
using System.Linq;
using System.Web.Mvc;
using RecruitmentManagementMVC.Models;

namespace RecruitmentManagementMVC.Controllers
{
    public class HomeController : Controller
    {
        private RecruitmentDbEntities db = new RecruitmentDbEntities();

        public ActionResult Index(string searchString, string location, int? page)
        {
            int pageSize = 6; // Mỗi trang 6 job
            int pageNumber = page ?? 1;

            var jobs = db.JobPosts.AsQueryable();

            // --- Bộ lọc tìm kiếm ---
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                jobs = jobs.Where(j => j.Title.Contains(searchString));
            }

            if (!string.IsNullOrWhiteSpace(location))
            {
                jobs = jobs.Where(j => j.Location.Contains(location));
            }

            // --- Sắp xếp mới nhất ---
            jobs = jobs.OrderByDescending(j => j.CreatedAt);

            // --- Phân trang thủ công ---
            int totalItems = jobs.Count();
            var jobsOnPage = jobs.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // --- Dữ liệu gửi sang View ---
            ViewBag.CurrentPage = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.SearchString = searchString;
            ViewBag.Location = location;

            return View(jobsOnPage);
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
}
