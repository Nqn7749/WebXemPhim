using System.Linq;
using System.Web.Mvc;
using WebXemPhim.Models;

namespace WebXemPhim.Controllers
{
    public class HistoryController : BaseController
    {
        public ActionResult Index()
        {
            if (Session["UserId"] == null) return RedirectToAction("Login", "Account");
            int userId = (int)Session["UserId"];

            var history = db.WatchHistories
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.LastWatched)
                .Select(h => h.Movie)
                .ToList();

            ViewBag.Title = "Lịch sử xem phim";
            return View(history);
        }

        // ✅ Xóa khỏi lịch sử
        [HttpPost]
        public ActionResult Remove(int movieId)
        {
            if (Session["UserId"] == null) return RedirectToAction("Login", "Account");
            int userId = (int)Session["UserId"];

            var his = db.WatchHistories.FirstOrDefault(h => h.UserId == userId && h.MovieId == movieId);
            if (his != null)
            {
                db.WatchHistories.DeleteOnSubmit(his);
                db.SubmitChanges();
            }

            return RedirectToAction("IndexHistory");
        }
    }
}
