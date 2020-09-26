using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Hra.Framework.Sample.Models
{
    public class CurrencyResponse
    {
        public string Ok { get; set; }
        [JsonPropertyName("data")]
        public Data Response { get; set; }
    }

    public class Pair
    {
        public string Symbol1 { get; set; }
        public string Symbol2 { get; set; }
        public double MinLotSize { get; set; }
        public double MinLotSizeS2 { get; set; }
        public object MaxLotSize { get; set; }
        public string MinPrice { get; set; }
        public string MaxPrice { get; set; }
    }

    public class Data
    {
        public List<Pair> Pairs { get; set; }
    }

    public class ResultItem
    {
        public string Timestamp { get; set; }
        public string Low { get; set; }
        public string High { get; set; }
        public string Last { get; set; }
        public string Volume { get; set; }
        public string Volume30d { get; set; }
        public double Bid { get; set; }
        public double Ask { get; set; }
        public string Error { get; set; }
    }
}
