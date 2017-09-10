using System.ComponentModel;
using Newtonsoft.Json;

namespace BoilerController.Common.Models
{
    public class Status : INotifyPropertyChanged
    {
        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("on_since")]
        public string OnSince { get; set; } 

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
