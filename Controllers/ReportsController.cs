using System.Linq;
using System.Web.Mvc;
using WebXemPhim.Models;

namespace WebXemPhim.Controllers
{
    public class ReportsController : Controller
    {
        private DataClasses1DataContext db;
        public ReportsController()
        {
            string connString = System.Configuration.ConfigurationManager
                .ConnectionStrings["MovieStreamingDBConnectionString"].ConnectionString;

            db = new DataClasses1DataContext(connString);
        }

        private bool IsAdmin()
        {
            return Session["UserRole"] != null && Session["UserRole"].ToString() == "Admin";
        }

        public ActionResult Index()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var topMovies = db.Ratings
                .GroupBy(r => r.MovieId)
                .Select(g => new {
                    MovieId = g.Key,
                    AvgRating = g.Average(r => r.Rating1),
                    Count = g.Count()
                })
                .OrderByDescending(x => x.AvgRating)
                .Take(5)
                .ToList();

            ViewBag.TopMovies = topMovies;
            return View();
        }
    }
}
