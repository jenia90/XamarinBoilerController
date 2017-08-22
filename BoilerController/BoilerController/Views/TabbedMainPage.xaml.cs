using BoilerController.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BoilerController.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabbedMainPage : TabbedPage
    {
        public TabbedMainPage ()
        {
            InitializeComponent();
        }
    }
}