using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using RecruitmentManagementMVC.Models;
using RecruitmentManagementMVC.Filters;

namespace RecruitmentManagementMVC.Controllers
{
    public class JobPostsController : Controller
    {
        private RecruitmentDbEntities db = new RecruitmentDbEntities();

        // ==================== DANH SÁCH JOB ====================
        public ActionResult Index()
        {
            var role = Session["Role"] as string;
            var userId = Session["UserId"] as int?;

            // Lấy tất cả job và include thông tin nhà tuyển dụng
            var jobs = db.JobPosts.Include(j => j.User)
                                  .OrderByDescending(j => j.CreatedAt)
                                  .ToList();

            ViewBag.Role = role;
            ViewBag.UserId = userId;

            return View(jobs);
        }

        // ==================== VIỆC ĐÃ TẠO (DÀNH CHO NHÀ TUYỂN DỤNG) ====================
        [AuthorizeRole("Recruiter", "Employer", "Admin")]
        public ActionResult MyJobs()
        {
            int? userId = Session["UserId"] as int?;
            if (!userId.HasValue)
            {
                TempData["Error"] = "Vui lòng đăng nhập để xem công việc của bạn.";
                return RedirectToAction("Login", "Account");
            }

            var jobs = db.JobPosts
                .Include(j => j.User)
                .Where(j => j.EmployerId == userId.Value)
                .OrderByDescending(j => j.CreatedAt)
                .ToList();

            ViewBag.Role = Session["Role"];
            ViewBag.UserId = userId;

            // Dùng lại view Index để hiển thị danh sách giống nhau
            return View("Index", jobs);
        }



        // ==================== CHI TIẾT JOB ====================
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var job = db.JobPosts.Include(j => j.User).FirstOrDefault(j => j.JobId == id);
            if (job == null)
                return HttpNotFound();

            return View(job);
        }

        // ==================== TẠO JOB (GET) ====================
        [AuthorizeRole("Recruiter", "Admin", "Employer")]
        public ActionResult Create()
        {
            return View();
        }

        // ==================== TẠO JOB (POST) ====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Recruiter", "Admin", "Employer")]
        public ActionResult Create([Bind(Include = "Title,Description,Location,Salary,ExpiryDate")] JobPost jobPost)
        {
            if (!ModelState.IsValid)
                return View(jobPost);

            int userId = Convert.ToInt32(Session["UserId"]);

            jobPost.EmployerId = userId;
            jobPost.CreatedAt = DateTime.Now;
            jobPost.UpdatedAt = DateTime.Now;
            jobPost.User = db.Users.Find(userId);

            db.JobPosts.Add(jobPost);
            db.SaveChanges();

            TempData["Success"] = "Đăng tin tuyển dụng thành công!";
            return RedirectToAction("Index");
        }

        // ==================== CHỈNH SỬA JOB (GET) ====================
        [AuthorizeRole("Recruiter", "Admin", "Employer")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var job = db.JobPosts.Find(id);
            if (job == null)
                return HttpNotFound();

            string role = Session["Role"] as string;
            int? userId = Session["UserId"] as int?;

            if ((role == "Recruiter" || role == "Employer") && job.EmployerId != userId)
            {
                TempData["Error"] = "Bạn không có quyền chỉnh sửa bài đăng này!";
                return RedirectToAction("Index");
            }

            return View(job);
        }

        // ==================== CHỈNH SỬA JOB (POST) ====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Recruiter", "Admin", "Employer")]
        public ActionResult Edit([Bind(Include = "JobId,Title,Description,Location,Salary,ExpiryDate")] JobPost jobPost)
        {
            if (!ModelState.IsValid)
                return View(jobPost);

            var existing = db.JobPosts.Find(jobPost.JobId);
            if (existing == null)
                return HttpNotFound();

            string role = Session["Role"] as string;
            int? userId = Session["UserId"] as int?;

            if ((role == "Recruiter" || role == "Employer") && existing.EmployerId != userId)
            {
                TempData["Error"] = "Bạn không có quyền chỉnh sửa bài đăng này!";
                return RedirectToAction("Index");
            }

            // ✅ Cập nhật thông tin
            existing.Title = jobPost.Title;
            existing.Description = jobPost.Description;
            existing.Location = jobPost.Location;
            existing.Salary = jobPost.Salary;
            existing.ExpiryDate = jobPost.ExpiryDate;
            existing.UpdatedAt = DateTime.Now;

            db.Entry(existing).State = EntityState.Modified;
            db.SaveChanges();

            TempData["Success"] = "Cập nhật bài đăng thành công!";
            return RedirectToAction("Index");
        }

        // ==================== XÓA JOB (GET) ====================
        [AuthorizeRole("Recruiter", "Admin", "Employer")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var job = db.JobPosts.Find(id);
            if (job == null)
                return HttpNotFound();

            string role = Session["Role"] as string;
            int? userId = Session["UserId"] as int?;

            if ((role == "Recruiter" || role == "Employer") && job.EmployerId != userId)
            {
                TempData["Error"] = "Bạn không có quyền xóa bài đăng này!";
                return RedirectToAction("Index");
            }

            return View(job);
        }

        // ==================== XÓA JOB (POST) ====================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Recruiter", "Admin", "Employer")]
        public ActionResult DeleteConfirmed(int id)
        {
            var job = db.JobPosts.Find(id);
            if (job == null)
                return HttpNotFound();

            string role = Session["Role"] as string;
            int? userId = Session["UserId"] as int?;

            if ((role == "Recruiter" || role == "Employer") && job.EmployerId != userId)
            {
                TempData["Error"] = "Bạn không có quyền xóa bài đăng này!";
                return RedirectToAction("Index");
            }

            try
            {
                // Xóa các Application liên quan trước
                var relatedApps = db.Applications.Where(a => a.JobId == id).ToList();
                if (relatedApps.Any())
                    db.Applications.RemoveRange(relatedApps);

                db.JobPosts.Remove(job);
                db.SaveChanges();

                TempData["Success"] = "Đã xóa bài đăng thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Không thể xóa bài đăng: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
        


        // ==================== GIẢI PHÓNG DB ====================
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}
