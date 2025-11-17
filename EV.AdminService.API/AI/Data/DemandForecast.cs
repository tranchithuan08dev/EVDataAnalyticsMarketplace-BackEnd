using Microsoft.ML.Data;

namespace EV.AdminService.API.AI.Data
{
    public class DemandForecast
    {
        [ColumnName("ForecastedValues")]
        public float[]? ForecastedValues { get; set; }
        [ColumnName("LowerBoundValues")]
        public float[]? LowerBoundValues { get; set; }
        [ColumnName("UpperBoundValues")]
        public float[]? UpperBoundValues { get; set; }
    }
}
