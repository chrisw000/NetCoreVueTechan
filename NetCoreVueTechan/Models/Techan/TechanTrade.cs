using System;

namespace NetCoreVueTechan.Models.Techan
{
    public struct TechanTrade
    {
        public long Date { get; private set; }
        public string Type { get; private set; }
        public decimal Price { get; private set; }
        public decimal Quantity { get; private set; }

        public TechanTrade(DateTime date, bool isBuy, decimal price, decimal quantity)
        {
            Date = (long) (date - Constants.UnixEpoch).TotalMilliseconds;
            Type = isBuy ? "buy" : "sell";
            Price = price;
            Quantity = quantity;
        }
    }
}
