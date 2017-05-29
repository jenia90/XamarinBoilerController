using System.ComponentModel;

namespace BoilerController.Models
{
    public class WeekDay : INotifyPropertyChanged
    {
        private string _day;
        private bool _isSelected;

        public string Day
        {
            get => _day;
            set
            {
                _day = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Day"));
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsSelected"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
