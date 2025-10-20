using System;
using System.Linq;
using System.Web.Mvc;
using RecruitmentManagementMVC.Models;
using RecruitmentManagementMVC.Filters;

namespace RecruitmentManagementMVC.Controllers
{
    [AuthorizeRole("Admin")]
    public class AdminController : Controller
    {
        private RecruitmentDbEntities db = new RecruitmentDbEntities();

        public ActionResult Index()
        {
            // ======= Tổng số người dùng =======
            var totalUsers = db.Users.Count();
            ViewBag.TotalUsers = totalUsers;

            // ======= Tổng số việc làm =======
            var totalJobs = db.JobPosts.Count();
            ViewBag.TotalJobs = totalJobs;

            // ======= Thống kê người dùng theo vai trò =======
            var userByRole = db.Users
                .GroupBy(u => u.Role)
                .Select(g => new { Role = g.Key, Count = g.Count() })
                .ToList();

            if (!userByRole.Any())
            {
                userByRole.Add(new { Role = "Không có dữ liệu", Count = 0 });
            }

            ViewBag.RoleLabels = string.Join(",", userByRole.Select(r => $"'{r.Role}'"));
            ViewBag.RoleCounts = string.Join(",", userByRole.Select(r => r.Count));

            // ======= Thống kê job theo tháng =======
            var jobList = db.JobPosts
                .Where(j => j.CreatedAt != null)
                .ToList();

            var jobByMonth = jobList
                .GroupBy(j => new
                {
                    Year = j.CreatedAt.Value.Year,
                    Month = j.CreatedAt.Value.Month
                })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            if (!jobByMonth.Any())
            {
                ViewBag.MonthLabels = "'Không có dữ liệu'";
                ViewBag.MonthCounts = "0";
            }
            else
            {
                ViewBag.MonthLabels = string.Join(",", jobByMonth.Select(m => $"'T{m.Month}/{m.Year}'"));
                ViewBag.MonthCounts = string.Join(",", jobByMonth.Select(m => m.Count));
            }

            // ======= Trả về view =======
            return View();
        }
    }
}
