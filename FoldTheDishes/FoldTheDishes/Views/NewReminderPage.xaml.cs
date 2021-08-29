using FoldTheDishes.Models;
using FoldTheDishes.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FoldTheDishes.Views
{
    public partial class NewReminderPage : ContentPage
    {
        public Reminder Item { get; set; }

        public NewReminderPage()
        {
            InitializeComponent();
            BindingContext = new NewReminderViewModel();
        }
    }
}