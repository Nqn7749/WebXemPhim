using System.Linq;
using System.Web.Mvc;
using WebXemPhim.Models;
using System.Configuration;

public class BaseController : Controller
{
    protected DataClasses1DataContext db;

    public BaseController()
    {
        string connString = ConfigurationManager
            .ConnectionStrings["MovieStreamingDBConnectionString"].ConnectionString;
        db = new DataClasses1DataContext(connString);
    }

    protected override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        // Luôn load danh sách thể loại cho layout
        ViewBag.Genres = db.Genres.OrderBy(g => g.GenreName).ToList();
        base.OnActionExecuting(filterContext);
    }
}
