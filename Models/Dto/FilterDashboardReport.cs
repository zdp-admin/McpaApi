using McpaApi.Models;

namespace McpaApi.Dto
{
    public class FilterDashboardReport
    {
        public required WebSite WebSite { get; set; }
        public required DateOnly StartDate { get; set; }
        public required DateOnly EndDate { get; set; }
        public DateTime Today { get; set; }
        public bool IsForReportEmail {get; set;} = false;
    }
}