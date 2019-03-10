using System;

namespace NetCoreVueTechan.Models.Techan
{
    public struct TechanTrendlineItem
    {
        public long Date { get; }

        public decimal? Value { get; }

        public TechanTrendlineItem(DateTime date, decimal? value)
        {
            Date = (long) (date - Constants.UnixEpoch).TotalMilliseconds;
            Value = value;
        }
    }
}