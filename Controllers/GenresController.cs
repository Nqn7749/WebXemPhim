using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebXemPhim.Models;

namespace WebXemPhim.Controllers
{
    public class GenresController : Controller
    {
        private DataClasses1DataContext db;

        public GenresController()
        {
            string connString = System.Configuration.ConfigurationManager
                .ConnectionStrings["MovieStreamingDBConnectionString"].ConnectionString;

            db = new DataClasses1DataContext(connString);
        }

        private bool IsAdmin()
        {
            return Session["UserRole"] != null && Session["UserRole"].ToString() == "Admin";
        }

        // Danh sách thể loại
        public ActionResult Index()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var genres = db.Genres.ToList();
            return View(genres ?? new List<Genre>()); // luôn trả về list, tránh null
        }

        // Thêm thể loại (GET)
        public ActionResult Create()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            return View();
        }

        // Thêm thể loại (POST)
        [HttpPost]
        public ActionResult Create(Genre genre)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            if (genre == null)
            {
                ViewBag.Error = "Dữ liệu không hợp lệ.";
                return View();
            }

            var existing = db.Genres.FirstOrDefault(g => g.GenreName == genre.GenreName);
            if (existing != null)
            {
                ViewBag.Error = "Tên thể loại đã tồn tại.";
                return View();
            }

            db.Genres.InsertOnSubmit(genre);
            db.SubmitChanges();

            return RedirectToAction("Index");
        }

        // Sửa thể loại (GET)
        public ActionResult Edit(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var genre = db.Genres.FirstOrDefault(g => g.GenreId == id);
            if (genre == null) return HttpNotFound();
            return View(genre);
        }

        // Sửa thể loại (POST)
        [HttpPost]
        public ActionResult Edit(Genre updatedGenre)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var genre = db.Genres.FirstOrDefault(g => g.GenreId == updatedGenre.GenreId);
            if (genre == null) return HttpNotFound();

            genre.GenreName = updatedGenre.GenreName;
            db.SubmitChanges();

            return RedirectToAction("Index");
        }

        // Xóa thể loại (GET)
        public ActionResult Delete(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var genre = db.Genres.FirstOrDefault(g => g.GenreId == id);
            if (genre == null) return HttpNotFound();
            return View(genre);
        }

        // POST (overload cùng tên Delete)
        [HttpPost]
        public ActionResult Delete(Genre model)
        {
            var genre = db.Genres.FirstOrDefault(g => g.GenreId == model.GenreId);
            if (genre == null) return HttpNotFound();

            db.Genres.DeleteOnSubmit(genre);
            db.SubmitChanges();
            return RedirectToAction("Index");
        }
    }
}
