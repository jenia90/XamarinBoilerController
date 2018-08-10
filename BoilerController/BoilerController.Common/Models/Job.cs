using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace BoilerController.Common.Models
{
    public class Job : INotifyPropertyChanged
    {

        private string _end;
        private Guid _deviceId;
        private string _start;
        private Guid _id;
        private string _type;
        private IEnumerable<DayOfWeek> _daysList;
        private string _deviceName;

        [JsonProperty("id")]
        public Guid Id
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

        [JsonProperty("deviceid")]
        public Guid DeviceId
        {
            get => _deviceId;
            set
            {
                if (value == _deviceId) return;
                _deviceId = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DeviceId"));
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
