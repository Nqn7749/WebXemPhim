using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebXemPhim.Models;

namespace WebXemPhim.Controllers
{
    public class MoviesController : Controller
    {
        private DataClasses1DataContext db;

        public MoviesController()
        {
            string connString = System.Configuration.ConfigurationManager
                .ConnectionStrings["MovieStreamingDBConnectionString"].ConnectionString;

            db = new DataClasses1DataContext(connString);
        }

        private bool IsAdmin()
        {
            return Session["UserRole"] != null && Session["UserRole"].ToString() == "Admin";
        }

        // 📋 Danh sách phim
        public ActionResult Index()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var movies = db.Movies.ToList();
            return View(movies ?? new List<Movie>());
        }

        // ➕ Thêm phim (GET)
        public ActionResult Create()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            ViewBag.Genres = db.Genres.ToList();
            return View(new Movie());
        }

        // ➕ Thêm phim (POST)
        [HttpPost]
        public ActionResult Create(Movie movie, int[] SelectedGenres)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            if (movie == null)
            {
                ViewBag.Error = "Dữ liệu không hợp lệ.";
                ViewBag.Genres = db.Genres.ToList();
                return View();
            }

            var existing = db.Movies.FirstOrDefault(m => m.Title == movie.Title);
            if (existing != null)
            {
                ViewBag.Error = "Tên phim đã tồn tại.";
                ViewBag.Genres = db.Genres.ToList();
                return View();
            }

            movie.CreatedAt = DateTime.Now;
            db.Movies.InsertOnSubmit(movie);
            db.SubmitChanges();

            // Gán thể loại
            if (SelectedGenres != null)
            {
                foreach (var genreId in SelectedGenres)
                {
                    db.MovieGenres.InsertOnSubmit(new MovieGenre
                    {
                        MovieId = movie.MovieId,
                        GenreId = genreId
                    });
                }
                db.SubmitChanges();
            }

            TempData["Success"] = "Thêm phim mới thành công!";
            return RedirectToAction("Index");
        }

        // ✏️ Sửa phim (GET)
        public ActionResult Edit(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var movie = db.Movies.FirstOrDefault(m => m.MovieId == id);
            if (movie == null) return HttpNotFound();

            ViewBag.Genres = db.Genres.ToList();
            ViewBag.SelectedGenres = db.MovieGenres
                .Where(mg => mg.MovieId == id)
                .Select(mg => mg.GenreId)
                .ToArray();

            return View(movie);
        }

        // ✏️ Sửa phim (POST)
        [HttpPost]
        public ActionResult Edit(Movie updatedMovie, int[] SelectedGenres)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var movie = db.Movies.FirstOrDefault(m => m.MovieId == updatedMovie.MovieId);
            if (movie == null) return HttpNotFound();

            movie.Title = updatedMovie.Title;
            movie.Description = updatedMovie.Description;
            movie.ReleaseYear = updatedMovie.ReleaseYear;
            movie.Country = updatedMovie.Country;
            movie.PosterUrl = updatedMovie.PosterUrl;
            movie.TrailerUrl = updatedMovie.TrailerUrl;
            movie.IsSeries = updatedMovie.IsSeries;
            movie.MovieUrl = updatedMovie.MovieUrl;

            // Cập nhật thể loại
            var oldGenres = db.MovieGenres.Where(mg => mg.MovieId == movie.MovieId);
            db.MovieGenres.DeleteAllOnSubmit(oldGenres);

            if (SelectedGenres != null)
            {
                foreach (var genreId in SelectedGenres)
                {
                    db.MovieGenres.InsertOnSubmit(new MovieGenre
                    {
                        MovieId = movie.MovieId,
                        GenreId = genreId
                    });
                }
            }

            db.SubmitChanges();

            TempData["Success"] = "Cập nhật phim thành công!";
            return RedirectToAction("Index");
        }

        // 🗑️ Xóa phim (GET)
        public ActionResult Delete(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var movie = db.Movies.FirstOrDefault(m => m.MovieId == id);
            if (movie == null) return HttpNotFound();
            return View(movie);
        }

        // 🗑️ Xóa phim (POST)
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var movie = db.Movies.FirstOrDefault(m => m.MovieId == id);
            if (movie == null) return HttpNotFound();

            var genres = db.MovieGenres.Where(mg => mg.MovieId == id);
            db.MovieGenres.DeleteAllOnSubmit(genres);

            var episodes = db.Episodes.Where(e => e.MovieId == id);
            db.Episodes.DeleteAllOnSubmit(episodes);

            db.Movies.DeleteOnSubmit(movie);
            db.SubmitChanges();

            TempData["Success"] = "Xóa phim thành công!";
            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            var movie = db.Movies.FirstOrDefault(m => m.MovieId == id);
            if (movie == null) return HttpNotFound();

            if (movie.IsSeries == true)
            {
                var episodes = db.Episodes
                    .Where(e => e.MovieId == id)
                    .OrderBy(e => e.EpisodeNumber)
                    .ToList();

                ViewBag.Episodes = episodes;
            }

            return View(movie);
        }

    }
}
