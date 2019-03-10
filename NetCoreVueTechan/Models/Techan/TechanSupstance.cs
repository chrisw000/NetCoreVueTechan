using System;

namespace NetCoreVueTechan.Models.Techan
{
    public struct TechanSupstance
    {
        public TechanSupstance(DateTime start, DateTime end, decimal? value)
        {
            Start = (long) (start - Constants.UnixEpoch).TotalMilliseconds;
            End = (long) (end - Constants.UnixEpoch).TotalMilliseconds;
            Value = value;
        }

        public long Start { get; }

        public long End { get; }

        public decimal? Value { get; }
    }
}