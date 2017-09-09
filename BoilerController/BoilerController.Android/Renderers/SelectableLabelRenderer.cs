using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BoilerController.Common.Controls;
using BoilerController.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(SelectableLabel), typeof(SelectableLabelRenderer))]
namespace BoilerController.Droid.Renderers
{
    class SelectableLabelRenderer : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                var label = (SelectableLabel)e.NewElement;
                var tapRecognizer = new TapGestureRecognizer();
                tapRecognizer.Tapped += (sender, ea) =>
                {
                    label.IsSelected = !label.IsSelected;
                    label.BackgroundColor = label.IsSelected ? label.SelectedColor : Color.Transparent;
                };
                label.GestureRecognizers.Add(tapRecognizer);
            }
        }
    }
}