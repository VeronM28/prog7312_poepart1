using prog7212_poe_part1_V1.Models;

namespace prog7212_poe_part1_V1.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalReports { get; set; }
        public int PendingReports { get; set; }
        public int InProgressReports { get; set; }
        public int CompletedReports { get; set; }
        public List<ReportModel> RecentReports { get; set; } = new List<ReportModel>();
        public List<CategoryStatistic> CategoryStatistics { get; set; } = new List<CategoryStatistic>();
    }
    public class CategoryStatistic
    {
        public string Category { get; set; }
        public int Count { get; set; }
    }
}
