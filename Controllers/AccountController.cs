using System.Linq;
using System.Web.Mvc;
using WebXemPhim.Models;

namespace WebXemPhim.Controllers
{
    public class AccountController : BaseController
    {


        // ====================================
        // LOGIN
        // ====================================
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            var user = db.Users.FirstOrDefault(u => u.Email == email && u.PasswordHash == password);

            if (user != null)
            {
                Session["UserId"] = user.UserId;
                Session["UserName"] = user.FullName;
                Session["UserRole"] = user.Role;

                return user.Role == "Admin"
                    ? RedirectToAction("Dashboard", "Admin")
                    : RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Email hoặc mật khẩu không đúng.";
            return View();
        }


        // ====================================
        // REGISTER
        // ====================================
        public ActionResult Register() => View();

        [HttpPost]
        public ActionResult Register(User newUser)
        {
            if (db.Users.Any(u => u.Email == newUser.Email))
            {
                ViewBag.Error = "Email đã tồn tại.";
                return View();
            }

            newUser.Role = "User";
            newUser.CreatedAt = System.DateTime.Now;

            db.Users.InsertOnSubmit(newUser);
            db.SubmitChanges();

            return RedirectToAction("Login");
        }


        // ====================================
        // LOGOUT
        // ====================================
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }



        // ====================================
        // USER PROFILE
        // ====================================
        public ActionResult UserProfile()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login");

            int uid = (int)Session["UserId"];

            var user = db.Users.FirstOrDefault(u => u.UserId == uid);

            return View(user);
        }


        // ====================================
        // EDIT PROFILE (GET)
        // ====================================
        [HttpGet]
        public ActionResult EditProfile()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login");

            int uid = (int)Session["UserId"];
            var user = db.Users.FirstOrDefault(u => u.UserId == uid);

            return PartialView(user);
        }



        // ====================================
        // EDIT PROFILE (POST)
        // ====================================
        [HttpPost]
        public ActionResult EditProfile(int UserId, string Email, string FullName)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login");

            var user = db.Users.FirstOrDefault(u => u.UserId == UserId);

            if (user == null)
            {
                TempData["Message"] = "❌ Không tìm thấy tài khoản.";
                return RedirectToAction("UserProfile");
            }

            // Kiểm tra trùng email
            if (db.Users.Any(u => u.Email == Email && u.UserId != UserId))
            {
                TempData["Message"] = "❌ Email đã tồn tại.";
                return RedirectToAction("UserProfile");
            }

            user.Email = Email;
            user.FullName = FullName;

            db.SubmitChanges();

            // Cập nhật lại session
            Session["UserName"] = user.FullName;

            TempData["Message"] = "✅ Cập nhật thông tin thành công.";
            return RedirectToAction("UserProfile");
        }



        // ====================================
        // CHANGE PASSWORD (GET)
        // ====================================
        [HttpGet]
        public ActionResult ChangePassword()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login");

            return PartialView();
        }


        // ====================================
        // CHANGE PASSWORD (POST)
        // ====================================
        [HttpPost]
        public ActionResult ChangePassword(string CurrentPassword, string NewPassword, string ConfirmPassword)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login");

            int uid = (int)Session["UserId"];
            var user = db.Users.FirstOrDefault(u => u.UserId == uid);

            if (user == null)
                return RedirectToAction("Login");

            if (user.PasswordHash != CurrentPassword)
            {
                TempData["Message"] = "❌ Mật khẩu hiện tại không đúng.";
                return RedirectToAction("UserProfile");
            }

            if (NewPassword != ConfirmPassword)
            {
                TempData["Message"] = "❌ Mật khẩu mới không khớp.";
                return RedirectToAction("UserProfile");
            }

            user.PasswordHash = NewPassword;
            db.SubmitChanges(); 

            TempData["Message"] = "✅ Đổi mật khẩu thành công.";
            return RedirectToAction("UserProfile");
        }
    }
}
