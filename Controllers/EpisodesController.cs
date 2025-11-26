using System;
using System.Linq;
using System.Web.Mvc;
using WebXemPhim.Models;

namespace WebXemPhim.Controllers
{
    public class EpisodesController : Controller
    {
        private DataClasses1DataContext db;

        public EpisodesController()
        {
            string connString = System.Configuration.ConfigurationManager
                .ConnectionStrings["MovieStreamingDBConnectionString"].ConnectionString;

            db = new DataClasses1DataContext(connString);
        }

        private bool IsAdmin()
        {
            return Session["UserRole"] != null && Session["UserRole"].ToString() == "Admin";
        }

        // 📋 Danh sách tập phim theo MovieId
        public ActionResult Index(int movieId)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var movie = db.Movies.FirstOrDefault(m => m.MovieId == movieId);
            if (movie == null || !movie.IsSeries.GetValueOrDefault()) return HttpNotFound();

            ViewBag.MovieTitle = movie.Title;
            ViewBag.MovieId = movieId;

            var episodes = db.Episodes
                .Where(e => e.MovieId == movieId)
                .OrderBy(e => e.EpisodeNumber)
                .ToList();

            return View(episodes);
        }

        // ➕ Thêm tập phim (GET)
        public ActionResult Create(int movieId)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var movie = db.Movies.FirstOrDefault(m => m.MovieId == movieId);
            if (movie == null || !movie.IsSeries.GetValueOrDefault()) return HttpNotFound();

            ViewBag.MovieTitle = movie.Title;
            ViewBag.MovieId = movieId;

            return View(new Episode { MovieId = movieId });
        }

        // ➕ Thêm tập phim (POST)
        [HttpPost]
        public ActionResult Create(Episode episode)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            if (episode == null || episode.MovieId == 0)
            {
                ViewBag.Error = "Dữ liệu không hợp lệ.";
                return View(episode);
            }

            db.Episodes.InsertOnSubmit(episode);
            db.SubmitChanges();

            TempData["Success"] = "Thêm tập phim thành công!";
            return RedirectToAction("Index", new { movieId = episode.MovieId });
        }

        // ✏️ Sửa tập phim (GET)
        public ActionResult Edit(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var episode = db.Episodes.FirstOrDefault(e => e.EpisodeId == id);
            if (episode == null) return HttpNotFound();

            ViewBag.MovieTitle = db.Movies.FirstOrDefault(m => m.MovieId == episode.MovieId)?.Title;
            return View(episode);
        }

        // ✏️ Sửa tập phim (POST)
        [HttpPost]
        public ActionResult Edit(Episode updated)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var episode = db.Episodes.FirstOrDefault(e => e.EpisodeId == updated.EpisodeId);
            if (episode == null) return HttpNotFound();

            episode.Title = updated.Title;
            episode.EpisodeNumber = updated.EpisodeNumber;
            episode.VideoUrl = updated.VideoUrl;
            episode.Duration = updated.Duration;

            db.SubmitChanges();

            TempData["Success"] = "Cập nhật tập phim thành công!";
            return RedirectToAction("Index", new { movieId = episode.MovieId });
        }

        // 🗑️ Xóa tập phim (GET)
        public ActionResult Delete(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var episode = db.Episodes.FirstOrDefault(e => e.EpisodeId == id);
            if (episode == null) return HttpNotFound();

            ViewBag.MovieTitle = db.Movies.FirstOrDefault(m => m.MovieId == episode.MovieId)?.Title;
            return View(episode);
        }

        // 🗑️ Xóa tập phim (POST)
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var episode = db.Episodes.FirstOrDefault(e => e.EpisodeId == id);
            if (episode == null) return HttpNotFound();

            int movieId = episode.MovieId;

            db.Episodes.DeleteOnSubmit(episode);
            db.SubmitChanges();

            TempData["Success"] = "Xóa tập phim thành công!";
            return RedirectToAction("Index", new { movieId });
        }
    }
}
