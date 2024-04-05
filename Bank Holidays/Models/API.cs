using System.Text.Json.Serialization;

namespace Bank_Holidays.Models
{
    public class EnglandAndWales
    {
        public string division { get; set; }
        public List<Event> events { get; set; }
    }

    public class Event
    {
        public string title { get; set; }
        public string date { get; set; }
        public string notes { get; set; }
        public bool bunting { get; set; }
    }

    public class NorthernIreland
    {
        public string division { get; set; }
        public List<Event> events { get; set; }
    }

    public class API
    {
        [JsonPropertyName("england-and-wales")]
        public EnglandAndWales englandandwales { get; set; }
        public Scotland scotland { get; set; }

        [JsonPropertyName("northern-ireland")]
        public NorthernIreland northernireland { get; set; }
    }

    public class Scotland
    {
        public string division { get; set; }
        public List<Event> events { get; set; }
    }
}