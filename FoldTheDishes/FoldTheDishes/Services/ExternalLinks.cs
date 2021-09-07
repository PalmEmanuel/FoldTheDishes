using Xamarin.Essentials;
using Xamarin.Forms;

namespace FoldTheDishes.Services
{
    public static partial class ExternalLinks
    {
        public static Command TwitterCommand = new Command(async () => await Browser.OpenAsync(Constants.TWITTER_URL));
    }
}
