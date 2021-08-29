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
        }
    }
}