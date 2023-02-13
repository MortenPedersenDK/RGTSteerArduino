using Newtonsoft.Json;

namespace RGTKeyboard
{
    public class ArduinoMessage
    {
        [JsonProperty("P")]
        public int Port { get; set; }

        [JsonProperty("A")]
        public int Activations { get; set; }

    }
}
