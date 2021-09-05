using FoldTheDishes.ViewModels;
using Xamarin.Forms;

namespace FoldTheDishes.Views
{
    public partial class RemindersPage : ContentPage
    {
        RemindersViewModel viewModel;

        public RemindersPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new RemindersViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.OnAppearing();
        }
    }
}