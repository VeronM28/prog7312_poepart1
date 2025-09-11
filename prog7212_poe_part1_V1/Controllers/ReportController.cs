using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using prog7212_poe_part1_V1.Data;
using prog7212_poe_part1_V1.Models;
using prog7212_poe_part1_V1.ViewModels;

namespace prog7212_poe_part1_V1.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<UserModel> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ReportController(ApplicationDbContext context, UserManager<UserModel> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Report/Create
        public IActionResult Create()
        {
            var model = new ReportViewModel
            {
                Categories = GetCategoryList()
            };
            return View(model);
        }

        // POST: Report/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReportViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                string? imagePath = null;

                // Handle image upload
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    imagePath = await SaveImageAsync(model.ImageFile);
                }

                var report = new ReportModel
                {
                    UserId = userId,
                    Category = model.Category,
                    Description = model.Description,
                    Location = model.Location,
                    ImagePath = imagePath,
                    DateSubmitted = DateTime.Now,
                    Status = "Pending"
                };

                _context.Reports.Add(report);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Report submitted successfully!";
                return RedirectToAction("Index", "Home");
            }

            model.Categories = GetCategoryList();
            return View(model);
        }

        // GET: Report/MyReports
        public async Task<IActionResult> MyReports()
        {
            var userId = _userManager.GetUserId(User);
            var reports = await _context.Reports
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.DateSubmitted)
                .ToListAsync();

            return View(reports);
        }

        private List<SelectListItem> GetCategoryList()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "-- Select Category --", Disabled = true, Selected = true },
                new SelectListItem { Value = "Sanitation", Text = "Sanitation" },
                new SelectListItem { Value = "Roads", Text = "Roads" },
                new SelectListItem { Value = "Utilities", Text = "Utilities" },
                new SelectListItem { Value = "Safety", Text = "Safety" },
                new SelectListItem { Value = "Environment", Text = "Environment" },
                new SelectListItem { Value = "Public Transport", Text = "Public Transport" },
                new SelectListItem { Value = "Housing", Text = "Housing" },
                new SelectListItem { Value = "Other", Text = "Other" }
            };
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            try
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "reports");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                return "/uploads/reports/" + fileName;
            }
            catch
            {
                // Log error here
                return null;
            }
        }
    }
}

