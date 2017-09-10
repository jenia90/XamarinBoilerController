using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace BoilerController.Common.Models
{
    public class Job : INotifyPropertyChanged
    {

        private string _end;
        private int _pin;
        private string _start;
        private int _id;
        private string _type;
        private IEnumerable<string> _daysList;
        private string _deviceName;

        [JsonProperty("id")]
        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ID"));
            }
        }

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

        [JsonProperty("dev")]
        public string DeviceName
        {
            get => _deviceName;
            set
            {
                if(value == _deviceName) return;
                _deviceName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DeviceName"));
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

        [JsonProperty("type")]
        public string Type
        {
            get => _type;
            set
            {
                if (value == _type) return;
                _type = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Type"));

            }
        }

        [JsonProperty("days")]
        public IEnumerable<string> DaysList
        {
            get => _daysList;
            set
            {
                if(value.Equals(_daysList)) return;
                _daysList = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DaysList"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
