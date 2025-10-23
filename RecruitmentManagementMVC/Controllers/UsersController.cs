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
    public class UsersController : Controller
    {
        private RecruitmentDbEntities db = new RecruitmentDbEntities();

        // ======================= INDEX + TÌM KIẾM + LỌC ==========================
        public ActionResult Index(string searchString, string roleFilter)
        {
            // Lấy toàn bộ user
            var users = db.Users.AsQueryable();

            // Lọc theo từ khóa (tên hoặc email)
            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(u =>
                    u.FullName.Contains(searchString) ||
                    u.Email.Contains(searchString));
            }

            // Lọc theo vai trò (Role)
            if (!String.IsNullOrEmpty(roleFilter) && roleFilter != "All")
            {
                users = users.Where(u => u.Role == roleFilter);
            }

            // Trả dữ liệu kèm danh sách role cho dropdown
            ViewBag.Roles = new List<string> { "All", "Admin", "Employer", "Candidate" };
            ViewBag.SearchString = searchString;
            ViewBag.RoleFilter = roleFilter;

            // Trả danh sách kết quả ra View
            return View(users.OrderByDescending(u => u.CreatedAt).ToList());
        }

        // ======================= CHI TIẾT NGƯỜI DÙNG ==========================
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = db.Users.Find(id);
            if (user == null)
                return HttpNotFound();

            return View(user);
        }

        // ======================= TẠO MỚI NGƯỜI DÙNG ==========================
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FullName,Email,PasswordHash,Role")] User user)
        {
            if (ModelState.IsValid)
            {
                user.CreatedAt = DateTime.Now;
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // ======================= CHỈNH SỬA NGƯỜI DÙNG ==========================
        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int currentUserId = (int)Session["UserId"];
            string currentRole = Session["Role"]?.ToString();

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = db.Users.Find(id);
            if (user == null)
                return HttpNotFound();

            // Nếu không phải Admin và muốn sửa người khác → chặn
            if (currentRole != "Admin" && user.UserId != currentUserId)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int currentUserId = (int)Session["UserId"];
            string currentRole = Session["Role"]?.ToString() ?? "";

            var existingUser = db.Users.Find(user.UserId);
            if (existingUser == null)
                return HttpNotFound();

            // ✅ Nếu KHÔNG phải admin và không phải chính chủ → chặn
            if (currentRole != "Admin" && existingUser.UserId != currentUserId)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            if (ModelState.IsValid)
            {
                // ✅ Chỉ cho admin sửa email & vai trò
                if (currentRole == "Admin")
                {
                    existingUser.FullName = user.FullName;
                    existingUser.Email = user.Email;
                    existingUser.Role = user.Role;
                    existingUser.PasswordHash = user.PasswordHash;
                }
                else
                {
                    // ✅ Người dùng chỉ được sửa tên & mật khẩu
                    existingUser.FullName = user.FullName;
                    existingUser.PasswordHash = user.PasswordHash;
                }

                // Giữ nguyên CreatedAt
                existingUser.CreatedAt = existingUser.CreatedAt;

                db.Entry(existingUser).State = EntityState.Modified;
                db.SaveChanges();

                // ✅ Sau khi lưu, điều hướng đúng nơi:
                if (currentRole == "Admin")
                    return RedirectToAction("Index");
                else
                    return RedirectToAction("Profile");
            }

            return View(user);
        }



        // ======================= XÓA NGƯỜI DÙNG ==========================
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = db.Users.Find(id);
            if (user == null)
                return HttpNotFound();

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // profile
        public ActionResult Profile()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int userId = (int)Session["UserId"];
            var user = db.Users.Find(userId);
            if (user == null)
                return RedirectToAction("Login", "Account");

            return View(user);
        }





        // ======================= DỌN DẸP ==========================
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
