using System.Linq;
using System.Web.Mvc;
using WebXemPhim.Models;

namespace WebXemPhim.Controllers
{
    public class ReportsController : BaseController
    {

        private bool IsAdmin()
        {
            return Session["UserRole"] != null && Session["UserRole"].ToString() == "Admin";
        }

        public ActionResult Index()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            // ✅ Top 5 phim được đánh giá cao nhất
            var topMovies = db.Ratings
                .GroupBy(r => r.MovieId)
                .Select(g => new {
                    MovieId = g.Key,
                    MovieTitle = db.Movies.FirstOrDefault(m => m.MovieId == g.Key).Title,
                    AvgRating = g.Average(r => r.Rating1),
                    Count = g.Count()
                })
                .OrderByDescending(x => x.AvgRating)
                .Take(5)
                .ToList();

            // ✅ Top 5 phim nhiều lượt xem nhất
            var topViews = db.WatchHistories
                .GroupBy(h => h.MovieId)
                .Select(g => new {
                    MovieId = g.Key,
                    MovieTitle = db.Movies.FirstOrDefault(m => m.MovieId == g.Key).Title,
                    Views = g.Count()
                })
                .OrderByDescending(x => x.Views)
                .Take(5)
                .ToList();

            // ✅ Top 5 phim nhiều lượt yêu thích nhất
            var topFavorites = db.Favorites
                .GroupBy(f => f.MovieId)
                .Select(g => new {
                    MovieId = g.Key,
                    MovieTitle = db.Movies.FirstOrDefault(m => m.MovieId == g.Key).Title,
                    Favorites = g.Count()
                })
                .OrderByDescending(x => x.Favorites)
                .Take(5)
                .ToList();

            ViewBag.TopMovies = topMovies;
            ViewBag.TopViews = topViews;
            ViewBag.TopFavorites = topFavorites;

            return View();
        }
    }
}
