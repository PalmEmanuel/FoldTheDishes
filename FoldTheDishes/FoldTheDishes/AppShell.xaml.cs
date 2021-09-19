﻿using FoldTheDishes.Views;
using System;
using Xamarin.Forms;

namespace FoldTheDishes
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            Device.SetFlags(new string[] { "AppTheme_Experimental" });
            InitializeComponent();
            Routing.RegisterRoute(nameof(ReminderDetailPage), typeof(ReminderDetailPage));
            Routing.RegisterRoute(nameof(NewReminderPage), typeof(NewReminderPage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
        }
    }
}
