using FoldTheDishes.ViewModels;
using Xamarin.Forms;

namespace FoldTheDishes.Views
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            BindingContext = new SettingsViewModel();
        }
    }
}