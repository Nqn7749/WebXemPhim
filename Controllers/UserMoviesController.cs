using System;
using System.Linq;
using System.Web.Mvc;
using WebXemPhim.Models;

namespace WebXemPhim.Controllers
{
    public class UserMoviesController : BaseController
    {
        private const int PageSize = 20;

        // ✅ Hàm phụ trợ phân trang
        private ActionResult PagedList(IQueryable<Movie> query, int page, string title)
        {
            var totalMovies = query.Count();
            var movies = query.Skip((page - 1) * PageSize).Take(PageSize).ToList();

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalMovies / PageSize);
            ViewBag.CurrentPage = page;
            ViewBag.Title = title;

            return View("List", movies);
        }

        // ✅ Trang chủ
        public ActionResult Index(int page = 1)
        {
            var query = db.Movies.OrderByDescending(m => m.CreatedAt);
            return PagedList(query, page, "Phim mới cập nhật");
        }

        // ✅ Phim lẻ
        public ActionResult Single(int page = 1)
        {
            var query = db.Movies.Where(m => !(m.IsSeries ?? false))
                                 .OrderByDescending(m => m.CreatedAt);
            return PagedList(query, page, "Phim lẻ");
        }

        // ✅ Phim bộ
        public ActionResult Series(int page = 1)
        {
            var query = db.Movies.Where(m => (m.IsSeries ?? false))
                                 .OrderByDescending(m => m.CreatedAt);
            return PagedList(query, page, "Phim bộ");
        }

        // ✅ Phim theo thể loại
        public ActionResult Genre(int id, int page = 1)
        {
            var query = db.MovieGenres.Where(mg => mg.GenreId == id)
                                      .Select(mg => mg.Movie)
                                      .OrderByDescending(m => m.CreatedAt);

            var genre = db.Genres.FirstOrDefault(g => g.GenreId == id);
            string title = genre != null ? $"Thể loại: {genre.GenreName}" : "Thể loại";

            return PagedList(query, page, title);
        }

        // ✅ Tìm kiếm phim
        public ActionResult Search(string query, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(query)) return RedirectToAction("Index");

            var movies = db.Movies.Where(m => m.Title.Contains(query))
                                  .OrderByDescending(m => m.CreatedAt);

            ViewBag.Query = query;
            return PagedList(movies, page, $"Kết quả tìm kiếm cho: {query}");
        }

        // ✅ Chi tiết phim
        public ActionResult Detail(int id)
        {
            var movie = db.Movies.FirstOrDefault(m => m.MovieId == id);
            if (movie == null) return HttpNotFound();

            if (movie.IsSeries ?? false)
            {
                ViewBag.Episodes = db.Episodes.Where(e => e.MovieId == id)
                                              .OrderBy(e => e.EpisodeNumber)
                                              .ToList();
            }

            var relatedGenres = db.MovieGenres.Where(g => g.MovieId == id)
                                              .Select(g => g.GenreId)
                                              .ToList();

            ViewBag.RelatedMovies = db.MovieGenres
                .Where(g => relatedGenres.Contains(g.GenreId) && g.MovieId != id)
                .Select(g => g.Movie)
                .Distinct()
                .Take(10)
                .ToList();

            return View(movie);
        }

        // ✅ Trang xem phim
        public ActionResult Watch(int id)
        {
            var episode = db.Episodes.FirstOrDefault(e => e.EpisodeId == id);
            Movie movie;

            if (episode != null)
            {
                movie = db.Movies.FirstOrDefault(m => m.MovieId == episode.MovieId);
                if (movie == null) return HttpNotFound();

                ViewBag.VideoUrl = episode.VideoUrl;
                ViewBag.CurrentEpisodeId = id;
                ViewBag.Episodes = db.Episodes.Where(e => e.MovieId == movie.MovieId)
                                              .OrderBy(e => e.EpisodeNumber)
                                              .ToList();
            }
            else
            {
                movie = db.Movies.FirstOrDefault(m => m.MovieId == id);
                if (movie == null) return HttpNotFound();

                ViewBag.VideoUrl = movie.MovieUrl;
                ViewBag.CurrentEpisodeId = null;

                if (movie.IsSeries ?? false)
                {
                    ViewBag.Episodes = db.Episodes.Where(e => e.MovieId == movie.MovieId)
                                                  .OrderBy(e => e.EpisodeNumber)
                                                  .ToList();
                }
            }

            var relatedGenres = db.MovieGenres.Where(g => g.MovieId == movie.MovieId)
                                              .Select(g => g.GenreId)
                                              .ToList();

            ViewBag.RelatedMovies = db.MovieGenres
                .Where(g => relatedGenres.Contains(g.GenreId) && g.MovieId != movie.MovieId)
                .Select(g => g.Movie)
                .Distinct()
                .Take(10)
                .ToList();

            return View(movie);
        }
    }
}
