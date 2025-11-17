namespace EV.AdminService.API.AI.Data
{
    public class AnalysisSummary
    {
        public long TotalTrips { get; set; } = 0;
        public double AverageKm { get; set; } = 0;
        public double MaxSoc { get; set; } = 0;
        public double MinBatteryTemp { get; set; } = 0;
        public int? PeakUsageHour { get; set; } = null;
    }
}
