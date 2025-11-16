using Microsoft.ML.Data;

namespace EV.AdminService.API.AI.Data
{
    public class TimeSeriesPrediction
    {
        [VectorType(3)]
        public double[] Prediction { get; set; } = null!;
    }
}
