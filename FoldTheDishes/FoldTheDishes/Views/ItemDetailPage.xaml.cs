using FoldTheDishes.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace FoldTheDishes.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}