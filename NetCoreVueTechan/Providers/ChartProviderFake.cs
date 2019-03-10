using System;
using System.Linq;
using NetCoreVueTechan.Models.Techan;

namespace NetCoreVueTechan.Providers
{
    public class ChartProviderFake : IChartProvider
    {
        private readonly Random _rng;
        private readonly TechanChartData _chart = new TechanChartData();

        public ChartProviderFake(Random rng)
        {
            _rng = rng;
            Initialize(15 * 24);
        }

        private void Initialize(int quantity)
        {
            var from = DateTime.Now
                .AddMinutes(-DateTime.Now.Minute).AddSeconds(-DateTime.Now.Second)
                .AddHours(-quantity);

            _chart.Name = $"Random Chart {from:G} to {DateTime.Now:G}";
            _chart.Symbol = "£";

            decimal? oldPrice = 100.0m;
            var volatility = 0.02m;
            
            // Add random OHLCV data
            for (var i = 0; i < quantity; i++)
            {

                var spread = 0m;
                var low = 1m;
                var high = 0m;

                for (var j = 0; j < 10; j++)
                {
                    spread = (decimal) _rng.NextDouble() - 0.5m;
                    
                    if (spread < low) low = spread;
                    if (spread > high) high = spread;
                }

                var newPrice = oldPrice * (1 + (volatility * spread));

                _chart.Ohlc.Add(new OhlcvDatapoint
                {
                    Open = oldPrice,
                    High = oldPrice * (1 + (volatility * high)),
                    Low = oldPrice * (1 + (volatility * low)),
                    Close = newPrice,
                    EndDateTime = from,
                    Volume = (500.0m * ((decimal)_rng.NextDouble()-0.5m)) + 250.0m
                });

                oldPrice = newPrice;
                from = from.AddHours(1);
            }

            // Add line for the Average High
            _chart.Supstances.Add(new TechanSupstance(_chart.Ohlc[5].EndDateTime, _chart.Ohlc[_chart.Ohlc.Count-5].EndDateTime, _chart.Ohlc.Select(o=>o.High).Average()));

            // Add line for the Average Low
            _chart.Supstances.Add(new TechanSupstance(_chart.Ohlc[5].EndDateTime, _chart.Ohlc[_chart.Ohlc.Count-5].EndDateTime, _chart.Ohlc.Select(o=>o.Low).Average()));
            
            // Add some trend lines
            _chart.Trendlines.Add(new TechanTrendline(new TechanTrendlineItem(_chart.Ohlc[8].EndDateTime, _chart.Ohlc[8].Low), new TechanTrendlineItem(_chart.Ohlc[54].EndDateTime, _chart.Ohlc[54].High)));
            _chart.Trendlines.Add(new TechanTrendline(new TechanTrendlineItem(_chart.Ohlc[99].EndDateTime, _chart.Ohlc[99].Low), new TechanTrendlineItem(_chart.Ohlc[187].EndDateTime, _chart.Ohlc[187].High)));
            _chart.Trendlines.Add(new TechanTrendline(new TechanTrendlineItem(_chart.Ohlc[242].EndDateTime, _chart.Ohlc[242].Low), new TechanTrendlineItem(_chart.Ohlc[347].EndDateTime, _chart.Ohlc[347].High)));

            // Add some trades
            _chart.Trades.Add(new TechanTrade(_chart.Ohlc[8].EndDateTime, true, _chart.Ohlc[8].Open.Value, 10));
            _chart.Trades.Add(new TechanTrade(_chart.Ohlc[54].EndDateTime, false, _chart.Ohlc[54].Open.Value, 10));

            _chart.Trades.Add(new TechanTrade(_chart.Ohlc[99].EndDateTime, true, _chart.Ohlc[99].Open.Value, 10));
            _chart.Trades.Add(new TechanTrade(_chart.Ohlc[187].EndDateTime, false, _chart.Ohlc[187].Open.Value, 10));

            _chart.Trades.Add(new TechanTrade(_chart.Ohlc[242].EndDateTime, true, _chart.Ohlc[242].Open.Value, 10));
            _chart.Trades.Add(new TechanTrade(_chart.Ohlc[347].EndDateTime, false, _chart.Ohlc[347].Open.Value, 10));
        }
        
        public TechanChartData GetChart(int overlayId)
        {
            // just return the fake... usually this would be off to the database for chart with overlayId
            return _chart;
        }

    }
}
