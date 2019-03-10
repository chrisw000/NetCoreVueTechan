using NetCoreVueTechan.Models.Techan;

namespace NetCoreVueTechan.Providers
{
    public interface IChartProvider
    {
        TechanChartData GetChart(int overlayId);
    }
}
