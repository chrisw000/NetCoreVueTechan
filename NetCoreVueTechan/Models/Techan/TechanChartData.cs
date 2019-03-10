using System.Collections.Generic;

namespace NetCoreVueTechan.Models.Techan
{
    public class TechanChartData
    {
        public IList<OhlcvDatapoint> Ohlc { get; } = new List<OhlcvDatapoint>();

        public IList<ValueDataPoint> Overlay { get; } = new List<ValueDataPoint>();

        public string Name { get; set; }

        public string Symbol { get; set; }

        public int Preroll { get; set; } = 1;
        public IList<TechanSupstance> Supstances { get;} = new List<TechanSupstance>();
        public IList<TechanTrendline> Trendlines { get; } = new List<TechanTrendline>();
        public IList<TechanTrade> Trades { get; } = new List<TechanTrade>();
    }
}
