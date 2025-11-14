namespace EV.AdminService.API.AI.Data
{
    public class CombinedAnalysisReport
    {
        public string QualitativeReport { get; set; } = string.Empty;
        public DemandForecast ForecastData { get; set; } = null!;
    }
}
