using System.Web.Mvc;
using WebXemPhim.Models;
using System.Linq;

namespace WebXemPhim.Controllers
{
    public class AdminController : BaseController
    {

        // Trang tổng quan quản trị
        public ActionResult Dashboard()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            ViewBag.TotalMovies = db.Movies.Count();
            ViewBag.TotalUsers = db.Users.Count();
            ViewBag.TotalRatings = db.Ratings.Count();
            ViewBag.TotalGenres = db.Genres.Count();

            return View();
        }

        // Kiểm tra quyền Admin
        private bool IsAdmin()
        {
            return Session["UserRole"] != null && Session["UserRole"].ToString() == "Admin";
        }
    }
}
