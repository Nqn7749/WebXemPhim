using System;
using System.Linq;
using System.Web.Mvc;
using WebXemPhim.Models;

namespace WebXemPhim.Controllers
{
    public class HomeController : BaseController
    {
        private const int PageSize = 20; // số phim mỗi trang

        public ActionResult Index(int page = 1)
        {


            //var slideshowMovies = db.Movies
            //    .OrderByDescending(m => m.CreatedAt)
            //    .Take(3)
            //    .ToList();
            //ViewBag.LatestMovies = slideshowMovies;


            var totalMovies = db.Movies.Count();
            var phimMoi = db.Movies
                .OrderByDescending(m => m.CreatedAt)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalMovies / PageSize);
            ViewBag.CurrentPage = page;
            ViewBag.Title = "Phim mới cập nhật";

            return View(phimMoi);
        }
    }
}
