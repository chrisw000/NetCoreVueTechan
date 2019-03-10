using System;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace NetCoreVueTechan.Models.Techan
{
    public class OhlcvDatapoint
    {
        [NotMapped]
        [JsonProperty(PropertyName = "date")]
        public long UnixEpochDate => (long) (EndDateTime - Constants.UnixEpoch).TotalMilliseconds;

        [JsonIgnore]
        public DateTime EndDateTime { get; set; }
        public decimal? Open { get; set; }
        public decimal? High { get; set; }
        public decimal? Low { get; set; }
        public decimal? Close { get; set; }
        public decimal? Volume { get; set; }
    }
}
