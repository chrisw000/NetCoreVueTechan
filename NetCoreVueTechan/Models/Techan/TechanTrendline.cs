namespace NetCoreVueTechan.Models.Techan
{
    public struct TechanTrendline
    {
        public TechanTrendlineItem Start { get; }
        public TechanTrendlineItem End { get; }

        public TechanTrendline(TechanTrendlineItem start, TechanTrendlineItem end)
        {
            Start = start;
            End = end;
        }
    }
}