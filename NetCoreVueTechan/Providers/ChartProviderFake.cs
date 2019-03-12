using System;
using System.Collections.Generic;
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
            var candlePeriodInSeconds = TimeSpan.FromHours(1).TotalSeconds;

            var from = DateTime.Now
                .AddMinutes(-DateTime.Now.Minute).AddSeconds(-DateTime.Now.Second)
                .AddSeconds(-quantity * candlePeriodInSeconds);

            _chart.Name = $"Random Chart {from:G} to {DateTime.Now:G}";
            _chart.Symbol = "£";

            decimal? oldPrice = 100.0m;
            var volatility = 0.02m;

            // Add sin wave "indicator" values
            _chart.Overlay = GenerateSinWaveIndicator(quantity, from, candlePeriodInSeconds);
            
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
                from = from.AddSeconds(candlePeriodInSeconds);
            }

            // Picks some random points to put trades / trends / support&resistance lines
            var p1a = (int)(45.0 - (_rng.NextDouble() * 40.0));
            var p1b = (int)(75.0 + (_rng.NextDouble() * 40.0));
            var p2a = (int)(165.0 - (_rng.NextDouble() * 40.0));
            var p2b = (int)(175.0 + (_rng.NextDouble() * 40.0));
            var p3a = (int)(285.0 - (_rng.NextDouble() * 40.0));
            var p3b = (int)(315.0 + (_rng.NextDouble() * 40.0));

            // Add line for the Average High / Average Low for the periods
            _chart.Supstances.Add(new TechanSupstance(_chart.Ohlc[p1a].EndDateTime, _chart.Ohlc[p1b].EndDateTime, _chart.Ohlc.ToList().GetRange(p1a, p1b-p1a).Select(o=>o.High).Average()));
            _chart.Supstances.Add(new TechanSupstance(_chart.Ohlc[p1a].EndDateTime, _chart.Ohlc[p1b].EndDateTime, _chart.Ohlc.ToList().GetRange(p1a, p1b-p1a).Select(o=>o.Low).Average()));
            _chart.Supstances.Add(new TechanSupstance(_chart.Ohlc[p2a].EndDateTime, _chart.Ohlc[p2b].EndDateTime, _chart.Ohlc.ToList().GetRange(p2a, p2b-p2a).Select(o=>o.High).Average()));
            _chart.Supstances.Add(new TechanSupstance(_chart.Ohlc[p2a].EndDateTime, _chart.Ohlc[p2b].EndDateTime, _chart.Ohlc.ToList().GetRange(p2a, p2b-p2a).Select(o=>o.Low).Average()));
            _chart.Supstances.Add(new TechanSupstance(_chart.Ohlc[p3a].EndDateTime, _chart.Ohlc[p3b].EndDateTime, _chart.Ohlc.ToList().GetRange(p3a, p3b-p3a).Select(o=>o.High).Average()));
            _chart.Supstances.Add(new TechanSupstance(_chart.Ohlc[p3a].EndDateTime, _chart.Ohlc[p3b].EndDateTime, _chart.Ohlc.ToList().GetRange(p3a, p3b-p3a).Select(o=>o.Low).Average()));
            
            // Add some trend lines
            _chart.Trendlines.Add(new TechanTrendline(new TechanTrendlineItem(_chart.Ohlc[p1a].EndDateTime, _chart.Ohlc[p1a].Low), new TechanTrendlineItem(_chart.Ohlc[p1b].EndDateTime, _chart.Ohlc[p1b].High)));
            _chart.Trendlines.Add(new TechanTrendline(new TechanTrendlineItem(_chart.Ohlc[p2a].EndDateTime, _chart.Ohlc[p2a].Low), new TechanTrendlineItem(_chart.Ohlc[p2b].EndDateTime, _chart.Ohlc[p2b].High)));
            _chart.Trendlines.Add(new TechanTrendline(new TechanTrendlineItem(_chart.Ohlc[p3a].EndDateTime, _chart.Ohlc[p3a].Low), new TechanTrendlineItem(_chart.Ohlc[p3b].EndDateTime, _chart.Ohlc[p3b].High)));

            // Add some trades
            _chart.Trades.Add(new TechanTrade(_chart.Ohlc[p1a].EndDateTime, true, _chart.Ohlc[p1a].Open.Value, 100));
            _chart.Trades.Add(new TechanTrade(_chart.Ohlc[p1b].EndDateTime, false, _chart.Ohlc[p1b].Open.Value, 100));

            _chart.Trades.Add(new TechanTrade(_chart.Ohlc[p2a].EndDateTime, true, _chart.Ohlc[p2a].Open.Value, 250));
            _chart.Trades.Add(new TechanTrade(_chart.Ohlc[p2b].EndDateTime, false, _chart.Ohlc[p2b].Open.Value, 250));

            _chart.Trades.Add(new TechanTrade(_chart.Ohlc[p3a].EndDateTime, true, _chart.Ohlc[p3a].Open.Value, 125));
            _chart.Trades.Add(new TechanTrade(_chart.Ohlc[p3b].EndDateTime, false, _chart.Ohlc[p3b].Open.Value, 125));
        }

        private static List<ValueDataPoint> GenerateSinWaveIndicator(int quantity, DateTime from, double periodInSeconds)
        {
            const double tau = 2 * Math.PI;
            const double amplitude = 100.0;
            const double frequency = 2.0;
            const int sampleRate = 360;

            const double theta = frequency * tau / sampleRate;
            
            var rc = new List<ValueDataPoint>();

            for (var i = 0; i < quantity; i++)
            {
                rc.Add(new ValueDataPoint()
                {
                    EndDateTime = from,
                    Value = (decimal)(amplitude * Math.Sin(theta * i))
                });

                from = from.AddSeconds(periodInSeconds);
            }

            return rc;
        }
        
        public TechanChartData GetChart(int overlayId)
        {
            // just return the fake... usually this would be off to the database for chart with overlayId
            return _chart;
        }
    }
}
