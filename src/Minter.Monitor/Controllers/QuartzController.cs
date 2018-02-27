using Microsoft.AspNetCore.Mvc;

namespace Minter.Monitor.Controllers
{
    public class QuartzController : Controller
    {
        // GET: Quartz
        public ActionResult Index()
        {
            return View();
        }
    }
}