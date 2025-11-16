using Microsoft.ML.Data;

namespace EV.AdminService.API.AI.Data
{
    public class EVDataPoint
    {
        [LoadColumn(0)]
        public string Timestamp { get; set; } = null!;
        [LoadColumn(1)]
        public float SoC { get; set; }
        [LoadColumn(2)]
        public float BatteryTemp { get; set; }
        [LoadColumn(3)]
        public float Odometer { get; set; }
        [LoadColumn(4)]
        public string DriverNotes { get; set; } = null!;
    }
}
