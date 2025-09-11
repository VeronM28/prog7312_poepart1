using prog7212_poe_part1_V1.Models;

namespace prog7212_poe_part1_V1.ViewModels
{
    public class AdminReportsViewModel
    {
        public List<ReportModel> Reports { get; set; } = new List<ReportModel>();
        public string SelectedCategory { get; set; } = "";
        public string SelectedStatus { get; set; } = "";
        public List<string> Categories { get; set; } = new List<string>();
        public List<string> Statuses { get; set; } = new List<string>();
    }
}
