using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RecruitmentManagementMVC.Models;
using System.Data.Entity;

namespace RecruitmentManagementMVC.Controllers
{
    public class JobApplicationsController : Controller
    {
        private RecruitmentDbEntities db = new RecruitmentDbEntities();

        // ======================== NỘP CV (GET) =========================
        [HttpGet]
        public ActionResult Apply(int jobId)
        {
            var job = db.JobPosts.Find(jobId);
            if (job == null)
                return HttpNotFound();

            ViewBag.JobTitle = job.Title;
            ViewBag.JobId = job.JobId;
            return View();
        }

        // ======================== NỘP CV (POST) =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Apply(int jobId, string coverLetter, HttpPostedFileBase CvFile)
        {
            var userId = Session["UserId"] as int?;
            if (userId == null)
                return RedirectToAction("Login", "Account");

            bool alreadyApplied = db.Applications.Any(a => a.JobId == jobId && a.CandidateId == userId);
            if (alreadyApplied)
            {
                TempData["Error"] = "⚠ Bạn đã nộp đơn cho công việc này rồi.";
                return RedirectToAction("Details", "JobPosts", new { id = jobId });
            }

            string cvPath = null;
            if (CvFile != null && CvFile.ContentLength > 0)
            {
                string ext = Path.GetExtension(CvFile.FileName).ToLower();
                if (ext != ".pdf" && ext != ".doc" && ext != ".docx")
                {
                    TempData["Error"] = "❌ Chỉ chấp nhận file PDF, DOC hoặc DOCX.";
                    return RedirectToAction("Apply", new { jobId });
                }

                string uploadsDir = Server.MapPath("~/Uploads/CV/");
                if (!Directory.Exists(uploadsDir))
                    Directory.CreateDirectory(uploadsDir);

                string uniqueName = $"{Path.GetFileNameWithoutExtension(CvFile.FileName)}_{Guid.NewGuid()}{ext}";
                CvFile.SaveAs(Path.Combine(uploadsDir, uniqueName));

                cvPath = "/Uploads/CV/" + uniqueName;
            }

            var application = new Application
            {
                JobId = jobId,
                CandidateId = userId.Value,
                CoverLetter = coverLetter,
                AppliedAt = DateTime.Now,
                CvFilePath = cvPath,
                Status = "Pending"
            };

            db.Applications.Add(application);
            db.SaveChanges();

            TempData["Success"] = "✅ Nộp đơn và CV thành công!";
            return RedirectToAction("MyApplications");
        }

        // ======================== ỨNG VIÊN XEM CÁC ĐƠN ĐÃ NỘP =========================
        public ActionResult MyApplications()
        {
            var userId = Session["UserId"] as int?;
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var apps = db.Applications
                .Include(a => a.JobPost)
                .Include(a => a.JobPost.User)  // Nhà tuyển dụng
                .Where(a => a.CandidateId == userId)
                .OrderByDescending(a => a.AppliedAt)
                .ToList();

            return View(apps);
        }

        // ======================== NHÀ TUYỂN DỤNG XEM ỨNG VIÊN =========================
        public ActionResult ApplicationsForMyJobs()
        {
            var employerId = Session["UserId"] as int?;
            if (employerId == null)
                return RedirectToAction("Login", "Account");

            var applications = db.Applications
                .Include(a => a.JobPost)
                .Include(a => a.User) // Ứng viên
                .Where(a => a.JobPost.EmployerId == employerId)
                .OrderByDescending(a => a.AppliedAt)
                .ToList();

            return View(applications);
        }

        // ======================== DUYỆT HỒ SƠ =========================
        [HttpPost]
        public ActionResult Approve(int id)
        {
            var app = db.Applications.Include(a => a.User).FirstOrDefault(a => a.ApplicationId == id);
            if (app != null)
            {
                app.Status = "Approved";
                db.SaveChanges();
                TempData["Success"] = $"✅ Đã duyệt hồ sơ của {app.User.FullName}.";
            }
            return RedirectToAction("ApplicationsForMyJobs");
        }

        // ======================== TỪ CHỐI HỒ SƠ =========================
        [HttpPost]
        public ActionResult Reject(int id)
        {
            var app = db.Applications.Include(a => a.User).FirstOrDefault(a => a.ApplicationId == id);
            if (app != null)
            {
                app.Status = "Rejected";
                db.SaveChanges();
                TempData["Info"] = $"❌ Đã từ chối hồ sơ của {app.User.FullName}.";
            }
            return RedirectToAction("ApplicationsForMyJobs");
        }
    }
}
