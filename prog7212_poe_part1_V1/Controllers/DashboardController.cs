using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using prog7212_poe_part1_V1.Models;

namespace prog7212_poe_part1_V1.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly UserManager<UserModel> _userManager;

        public DashboardController(UserManager<UserModel> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.UserName = user?.FullName ?? user?.Email ?? "User";
            
            return View();
        }
    }
}
