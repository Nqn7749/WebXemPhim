using System.Linq;
using System.Web.Mvc;
using WebXemPhim.Models;

namespace WebXemPhim.Controllers
{
    public class FavoritesController : BaseController
    {
        public ActionResult Index()
        {
            if (Session["UserId"] == null) return RedirectToAction("Login", "Account");
            int userId = (int)Session["UserId"];

            var favorites = db.Favorites
                .Where(f => f.UserId == userId)
                .Select(f => f.Movie)
                .OrderByDescending(m => m.CreatedAt)
                .ToList();

            ViewBag.Title = "Danh sách phim yêu thích";
            return View(favorites);
        }

        // ✅ Xóa khỏi yêu thích
        [HttpPost]
        public ActionResult Remove(int movieId)
        {
            if (Session["UserId"] == null) return RedirectToAction("Login", "Account");
            int userId = (int)Session["UserId"];

            var fav = db.Favorites.FirstOrDefault(f => f.UserId == userId && f.MovieId == movieId);
            if (fav != null)
            {
                db.Favorites.DeleteOnSubmit(fav);
                db.SubmitChanges();
            }

            return RedirectToAction("IndexFavorite");
        }
    }
}
