using System.Linq;
using System.Web.Mvc;
using WebXemPhim.Models; // namespace chứa DataClasses1DataContext

namespace WebXemPhim.Controllers
{
    public class HomeController : Controller
    {
        private DataClasses1DataContext db;

        public HomeController()
        {
            // Lấy connection string từ Web.config
            string connString = System.Configuration.ConfigurationManager
                .ConnectionStrings["MovieStreamingDBConnectionString"].ConnectionString;

            db = new DataClasses1DataContext(connString);
        }

        public ActionResult Index()
        {
            var phimMoi = db.Movies
                .OrderByDescending(m => m.CreatedAt)
                .Take(12)
                .ToList();

            return View(phimMoi);
        }
    }
}
