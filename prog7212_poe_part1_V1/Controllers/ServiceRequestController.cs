using Microsoft.AspNetCore.Mvc;

namespace prog7212_poe_part1_V1.Controllers
{
    public class ServiceRequestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
