using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace BoilerController.Models
{
    public class Job : INotifyPropertyChanged
    {
        private string _end;
        private int _pin;
        private string _start;

        [JsonProperty("end")]
        public string End
        {
            get => _end;
            set
            {
                if (value == _end) return;
                _end = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("End"));
            }
        }

        [JsonProperty("pin")]
        public int Pin
        {
            get => _pin;
            set
            {
                if (value == _pin) return;
                _pin = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Pin"));
            }
        }

        [JsonProperty("start")]
        public string Start
        {
            get => _start;
            set
            {
                if (value == _start) return;
                _start = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Start"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
