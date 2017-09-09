using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BoilerController.Common.Controls;
using BoilerController.Common.Models;
using BoilerController.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using View = Xamarin.Forms.View;

[assembly:ExportRenderer(typeof(DaySelection), typeof(DaySelectionRenderer))]
namespace BoilerController.Droid.Renderers
{
    class DaySelectionRenderer : ViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                var sel = (DaySelection) e.NewElement;
                sel.Days = new ObservableCollection<WeekDay>();
            }
        }
    }
}