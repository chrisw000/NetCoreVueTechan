using Microsoft.AspNetCore.Mvc;
using NetCoreVueTechan.Providers;
using NetCoreVueTechan.Models.Techan;

namespace NetCoreVueTechan.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class OverlayController : ControllerBase
    {
        private readonly IChartProvider _chartProvider;

        public OverlayController(IChartProvider chartProvider)
        {
            _chartProvider = chartProvider;
        }

        /// <summary>
        /// Gets the data required to build the TechanJs Chart
        /// </summary>
        /// <param name="overlayParameterId"></param>
        /// <returns></returns>
        [HttpGet("chartByOverlay/{overlayParameterId:int}", Name = "GetTechanChartDataByOverlay")]
        [ProducesResponseType(404)]
        public ActionResult<TechanChartData> ChartDataByOverlay(int overlayParameterId)
        {
            return _chartProvider.GetChart(overlayParameterId);
        }
    }
}
