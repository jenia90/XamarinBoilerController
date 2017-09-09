using Xamarin.Forms;

namespace BoilerController.Common.Controls
{
    public class SelectableLabel : Label
    {
        /// <summary>
        /// Is selected property
        /// </summary>
        public static readonly BindableProperty IsSelectedProperty =
            BindableProperty.Create("IsSelected", typeof(bool), typeof(SelectableLabel), false);

        public static readonly BindableProperty SelectedColorProperty =
            BindableProperty.Create("SelectedColor", typeof(Color), typeof(SelectableLabel), Color.Accent);

        public bool IsSelected
        {
            get => (bool) GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public Color SelectedColor
        {
            get => (Color) GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }
    }
}