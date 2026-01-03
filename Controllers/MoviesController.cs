using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.Mvc;
using WebXemPhim.Models;
using System.Data.Linq;

namespace WebXemPhim.Controllers
{
    public class MoviesController : BaseController
    {

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
        public ActionResult Create(Movie movie, int[] SelectedGenres, HttpPostedFileBase PosterFile)
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

            if (PosterFile != null && PosterFile.ContentLength > 0)
            {
                string fileName = Guid.NewGuid() + System.IO.Path.GetExtension(PosterFile.FileName);
                string path = Server.MapPath("~/Uploads/Posters/" + fileName);

                PosterFile.SaveAs(path);

                movie.PosterUrl = "/Uploads/Posters/" + fileName;
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
        public ActionResult Edit(Movie updatedMovie, int[] SelectedGenres, HttpPostedFileBase PosterFile)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var movie = db.Movies.FirstOrDefault(m => m.MovieId == updatedMovie.MovieId);
            if (movie == null) return HttpNotFound();

            string oldPoster = movie.PosterUrl;
            // Upload ảnh mới nếu có
            if (PosterFile != null && PosterFile.ContentLength > 0)
            {
                string fileName = Guid.NewGuid() + System.IO.Path.GetExtension(PosterFile.FileName);
                string path = Server.MapPath("~/Uploads/Posters/" + fileName);

                PosterFile.SaveAs(path);

                movie.PosterUrl = "/Uploads/Posters/" + fileName;
            }
            else
            {
                movie.PosterUrl = oldPoster; 
            }

            movie.Title = updatedMovie.Title;
            movie.Description = updatedMovie.Description;
            movie.ReleaseYear = updatedMovie.ReleaseYear;
            movie.Country = updatedMovie.Country;
            
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

            ViewBag.Genres = db.MovieGenres
                .Where(mg => mg.MovieId == id)
                .Select(mg => mg.Genre)
                .ToList();

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
