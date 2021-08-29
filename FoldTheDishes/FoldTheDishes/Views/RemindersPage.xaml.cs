using FoldTheDishes.ViewModels;
using Xamarin.Forms;

namespace FoldTheDishes.Views
{
    public partial class RemindersPage : ContentPage
    {
        RemindersViewModel _viewModel;

        public RemindersPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new RemindersViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}