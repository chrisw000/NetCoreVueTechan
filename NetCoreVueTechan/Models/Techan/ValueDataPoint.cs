using System;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace NetCoreVueTechan.Models.Techan
{
    public class ValueDataPoint
    {
        [NotMapped]
        [JsonProperty(PropertyName = "date")]
        public long UnixEpochDate => (long) (EndDateTime - Constants.UnixEpoch).TotalMilliseconds;

        [JsonIgnore]
        public DateTime EndDateTime { get; set; }
        public decimal? Value { get; set; }
    }
}