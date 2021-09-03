using FoldTheDishes.Models;
using FoldTheDishes.ViewModels;
using Xamarin.Forms;

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