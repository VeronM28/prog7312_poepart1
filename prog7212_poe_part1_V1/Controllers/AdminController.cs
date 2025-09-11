using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prog7212_poe_part1_V1.Data;
using prog7212_poe_part1_V1.Models;
using prog7212_poe_part1_V1.ViewModels;

namespace prog7212_poe_part1_V1.Controllers
{
    [Authorize(Roles = "Admin")] // Ensure only admins can access
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<UserModel> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<UserModel> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Admin/Reports
        public async Task<IActionResult> Reports(string category = "", string status = "")
        {
            var query = _context.Reports.Include(r => r.User).AsQueryable();

            // Filter by category if specified
            if (!string.IsNullOrEmpty(category) && category != "All")
            {
                query = query.Where(r => r.Category == category);
            }

            // Filter by status if specified
            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                query = query.Where(r => r.Status == status);
            }

            var reports = await query.OrderByDescending(r => r.DateSubmitted).ToListAsync();

            var viewModel = new AdminReportsViewModel
            {
                Reports = reports,
                SelectedCategory = category,
                SelectedStatus = status,
                Categories = GetCategoryList(),
                Statuses = GetStatusList()
            };

            return View(viewModel);
        }

        // POST: Admin/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int reportId, string status)
        {
            try
            {
                var report = await _context.Reports.FindAsync(reportId);
                if (report == null)
                {
                    TempData["Error"] = "Report not found.";
                    return RedirectToAction("Reports");
                }

                // Validate status
                var validStatuses = new[] { "Pending", "In-Progress", "Completed" };
                if (!validStatuses.Contains(status))
                {
                    TempData["Error"] = "Invalid status.";
                    return RedirectToAction("Reports");
                }

                report.Status = status;
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Report #{report.Id} status updated to {status}.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while updating the status.";
                // Log the exception here
            }

            return RedirectToAction("Reports");
        }

        // GET: Admin/ReportDetails/5
        public async Task<IActionResult> ReportDetails(int id)
        {
            var report = await _context.Reports
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (report == null)
            {
                TempData["Error"] = "Report not found.";
                return RedirectToAction("Reports");
            }

            return View(report);
        }

        private List<string> GetCategoryList()
        {
            return new List<string>
            {
                "All",
                "Sanitation",
                "Roads",
                "Utilities",
                "Safety",
                "Environment",
                "Public Transport",
                "Housing",
                "Other"
            };
        }

        private List<string> GetStatusList()
        {
            return new List<string>
            {
                "All",
                "Pending",
                "In-Progress",
                "Completed"
            };
        }

        // GET: Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var totalReports = await _context.Reports.CountAsync();
            var pendingReports = await _context.Reports.CountAsync(r => r.Status == "Pending");
            var inProgressReports = await _context.Reports.CountAsync(r => r.Status == "In-Progress");
            var completedReports = await _context.Reports.CountAsync(r => r.Status == "Completed");

            var recentReports = await _context.Reports
                .Include(r => r.User)
                .OrderByDescending(r => r.DateSubmitted)
                .Take(5)
                .ToListAsync();

            var categoryStats = await _context.Reports
                .GroupBy(r => r.Category)
                .Select(g => new CategoryStatistic
                {
                    Category = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(cs => cs.Count)
                .ToListAsync();

            var dashboardViewModel = new AdminDashboardViewModel
            {
                TotalReports = totalReports,
                PendingReports = pendingReports,
                InProgressReports = inProgressReports,
                CompletedReports = completedReports,
                RecentReports = recentReports,
                CategoryStatistics = categoryStats
            };

            return View(dashboardViewModel);
        }
    }
}
