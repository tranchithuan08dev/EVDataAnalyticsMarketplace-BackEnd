using EV.AdminService.API.AI.Data;

namespace EV.AdminService.API.AI.DataMap
{
    public sealed class EVDataPointMap : CsvHelper.Configuration.ClassMap<EVDataPoint>
    {
        public EVDataPointMap()
        {
            Map(m => m.Timestamp).Index(0); Map(m => m.SoC).Index(1);
            Map(m => m.BatteryTemp).Index(2); Map(m => m.Odometer).Index(3);
            Map(m => m.DriverNotes).Index(4);
        }
    }
}
