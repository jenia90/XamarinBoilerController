﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoilerController.Common.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BoilerController.Common.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DaySelection : ContentView
    {
        public DaySelection()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty DaysProperty =
            BindableProperty.Create("Days", typeof(ObservableCollection<WeekDay>), typeof(DaySelection));

        public ObservableCollection<WeekDay> Days
        {
            get => (ObservableCollection<WeekDay>)GetValue(DaysProperty);
            set => SetValue(DaysProperty, value);
        }
    }
}