using FoldTheDishes.ViewModels;
using Xamarin.Forms;

namespace FoldTheDishes.Views
{
    public partial class RemindersPage : TabbedPage
    {
        RemindersViewModel viewModel;

        public RemindersPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new RemindersViewModel();

            CurrentPageChanged += WhenCurrentPageChanged;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.OnAppearing();
        }

        void WhenCurrentPageChanged(object sender, System.EventArgs e)
        {
            //viewModel.CurrentPage = CurrentPage.Title;
            viewModel.LoadRemindersCommand.Execute(null);
        }
    }
}