using FoldTheDishes.ViewModels;
using Xamarin.Forms;

namespace FoldTheDishes.Views
{
    public partial class NewReminderPage : ContentPage
    {
        public NewReminderPage()
        {
            InitializeComponent();
            BindingContext = new NewReminderViewModel();
        }
    }
}