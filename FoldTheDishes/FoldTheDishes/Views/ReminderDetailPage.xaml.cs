using FoldTheDishes.ViewModels;
using Xamarin.Forms;

namespace FoldTheDishes.Views
{
    public partial class ReminderDetailPage : ContentPage
    {
        public ReminderDetailPage()
        {
            InitializeComponent();
            BindingContext = new ReminderDetailViewModel();

            // https://stackoverflow.com/a/59126960/4946729
            Shell.SetBackButtonBehavior(this, new BackButtonBehavior
            {
                Command = new Command(((ReminderDetailViewModel)BindingContext).CustomBackButtonAction)
            });
        }
    }
}