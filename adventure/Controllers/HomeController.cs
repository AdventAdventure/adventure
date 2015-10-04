using System.Web.Mvc;

namespace Adventure.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return File(Server.MapPath("~") + "index.html", "text/html");
        }
    }
}