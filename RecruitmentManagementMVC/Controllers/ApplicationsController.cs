using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RecruitmentManagementMVC.Models;

namespace RecruitmentManagementMVC.Controllers
{
    public class ApplicationsController : Controller
    {
        private RecruitmentDbEntities db = new RecruitmentDbEntities();

        // ===================== INDEX (DANH SÁCH + TÌM KIẾM + LỌC) =====================
        public ActionResult Index(string searchString, string jobFilter, string employerFilter)
        {
            var applications = db.Applications
                .Include(a => a.User)
                .Include(a => a.JobPost)
                .Include(a => a.JobPost.User);

            // 🔍 Tìm kiếm theo tên ứng viên hoặc tiêu đề job
            if (!string.IsNullOrEmpty(searchString))
            {
                applications = applications.Where(a =>
                    a.User.FullName.Contains(searchString) ||
                    a.JobPost.Title.Contains(searchString));
            }

            // 🧾 Lọc theo Job
            if (!string.IsNullOrEmpty(jobFilter))
            {
                applications = applications.Where(a => a.JobPost.Title == jobFilter);
            }

            // 🧾 Lọc theo Nhà tuyển dụng
            if (!string.IsNullOrEmpty(employerFilter))
            {
                applications = applications.Where(a => a.JobPost.User.FullName == employerFilter);
            }

            // 📋 Đổ dữ liệu dropdown
            ViewBag.JobList = db.JobPosts.Select(j => j.Title).Distinct().ToList();
            ViewBag.EmployerList = db.Users
                .Where(u => u.Role == "Recruiter" || u.Role == "Employer")
                .Select(u => u.FullName)
                .Distinct()
                .ToList();

            // Giữ giá trị tìm kiếm
            ViewBag.SearchString = searchString;
            ViewBag.JobFilter = jobFilter;
            ViewBag.EmployerFilter = employerFilter;

            return View(applications.ToList());
        }

        // ===================== CHI TIẾT =====================
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var application = db.Applications
                .Include(a => a.User)
                .Include(a => a.JobPost.User) // thêm dòng này để load chủ job
                .FirstOrDefault(a => a.ApplicationId == id);

            if (application == null)
                return HttpNotFound();

            return View(application);
        }


        // ===================== TẠO MỚI =====================
        public ActionResult Create()
        {
            ViewBag.CandidateId = new SelectList(db.Users, "UserId", "FullName");
            ViewBag.JobId = new SelectList(db.JobPosts, "JobId", "Title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ApplicationId,JobId,CandidateId,CoverLetter,AppliedAt")] Application application)
        {
            if (ModelState.IsValid)
            {
                application.AppliedAt = application.AppliedAt ?? DateTime.Now;
                db.Applications.Add(application);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CandidateId = new SelectList(db.Users, "UserId", "FullName", application.CandidateId);
            ViewBag.JobId = new SelectList(db.JobPosts, "JobId", "Title", application.JobId);
            return View(application);
        }

        // ===================== CHỈNH SỬA =====================
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var application = db.Applications.Find(id);
            if (application == null)
                return HttpNotFound();

            ViewBag.CandidateId = new SelectList(db.Users, "UserId", "FullName", application.CandidateId);
            ViewBag.JobId = new SelectList(db.JobPosts, "JobId", "Title", application.JobId);
            return View(application);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ApplicationId,JobId,CandidateId,CoverLetter,AppliedAt")] Application application)
        {
            if (ModelState.IsValid)
            {
                application.AppliedAt = application.AppliedAt ?? DateTime.Now;
                db.Entry(application).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CandidateId = new SelectList(db.Users, "UserId", "FullName", application.CandidateId);
            ViewBag.JobId = new SelectList(db.JobPosts, "JobId", "Title", application.JobId);
            return View(application);
        }

        // ===================== XÓA =====================
        // GET: Applications/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var application = db.Applications
                .Include(a => a.User)
                .Include(a => a.JobPost)
                .FirstOrDefault(a => a.ApplicationId == id);

            if (application == null)
                return HttpNotFound();

            return View(application);
        }

        // POST: Applications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var application = db.Applications.Find(id);
            if (application != null)
            {
                db.Applications.Remove(application);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }


        // ===================== DỌN DẸP =====================
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}
