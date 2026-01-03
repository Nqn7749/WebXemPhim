using System.Linq;
using System.Web.Mvc;
using WebXemPhim.Models;

namespace WebXemPhim.Controllers
{
    public class UsersController : BaseController
    {

        private bool IsAdmin()
        {
            return Session["UserRole"] != null && Session["UserRole"].ToString() == "Admin";
        }

        // Danh sách người dùng
        public ActionResult Index()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var users = db.Users.ToList();
            return View(users);
        }

        // Thêm người dùng (GET)
        public ActionResult Create()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            return View();
        }

        // Thêm người dùng (POST)
        [HttpPost]
        public ActionResult Create(User user)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var existing = db.Users.FirstOrDefault(u => u.Email == user.Email);
            if (existing != null)
            {
                ViewBag.Error = "Email đã tồn tại.";
                return View();
            }

            user.CreatedAt = System.DateTime.Now;
            db.Users.InsertOnSubmit(user);
            db.SubmitChanges();

            return RedirectToAction("Index");
        }

        // Sửa người dùng (GET)
        public ActionResult Edit(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var user = db.Users.FirstOrDefault(u => u.UserId == id);
            if (user == null) return HttpNotFound();
            return View(user);
        }

        // Sửa người dùng (POST)
        [HttpPost]
        public ActionResult Edit(User updatedUser)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var user = db.Users.FirstOrDefault(u => u.UserId == updatedUser.UserId);
            if (user == null) return HttpNotFound();

            user.FullName = updatedUser.FullName;
            user.Email = updatedUser.Email;
            user.PasswordHash = updatedUser.PasswordHash;
            user.Role = updatedUser.Role;
            user.AvatarUrl = updatedUser.AvatarUrl;

            db.SubmitChanges();
            return RedirectToAction("Index");
        }

        // Xóa người dùng (GET)
        public ActionResult Delete(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var user = db.Users.FirstOrDefault(u => u.UserId == id);
            if (user == null) return HttpNotFound();
            return View(user);
        }

        // Xóa người dùng (POST)
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var user = db.Users.FirstOrDefault(u => u.UserId == id);
            if (user == null) return HttpNotFound();

            db.Users.DeleteOnSubmit(user);
            db.SubmitChanges();

            return RedirectToAction("Index");
        }
    }
}
