using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.App;
using FoldTheDishes.Services;
using FoldTheDishes.ViewModels;
using FoldTheDishes.Views;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace FoldTheDishes.Droid
{
    [Activity(Label = "fold the dishes", Theme = "@style/MainTheme",
        LaunchMode = LaunchMode.SingleTop, Icon = "@mipmap/ic_launcher", RoundIcon = "@mipmap/ic_launcher_round",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //// Hide status bar
            //Window.AddFlags(WindowManagerFlags.Fullscreen | WindowManagerFlags.TurnScreenOn);
            //if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            //{
            //    var stBarHeight = typeof(FormsAppCompatActivity).GetField("statusBarHeight", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            //    if (stBarHeight == null)
            //    {
            //        stBarHeight = typeof(FormsAppCompatActivity).GetField("_statusBarHeight", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            //    }
            //    stBarHeight?.SetValue(this, 0);
            //}

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightFollowSystem;

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Forms.Init(this, savedInstanceState);
            FormsMaterial.Init(this, savedInstanceState);
            LoadApplication(new App());
            CreateNotificationFromIntent(Intent);
        }

        protected override void OnNewIntent(Intent intent)
        {
            CreateNotificationFromIntent(intent);
        }

        void CreateNotificationFromIntent(Intent intent)
        {
            if (intent?.Extras != null)
            {
                int id = intent.GetIntExtra(AndroidNotificationManager.IdKey, -1);
                string text = intent.GetStringExtra(AndroidNotificationManager.TextKey);
                DependencyService.Get<INotificationManager>().ReceiveNotification(id, text);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        // https://theconfuzedsourcecode.wordpress.com/2017/03/12/lets-override-navigation-bar-back-button-click-in-xamarin-forms/
        public override void OnBackPressed()
        {
            // this is not necessary, but in Android user 
            // has both Nav bar back button and
            // physical back button its safe 
            // to cover the both events

            // retrieve the current xamarin forms page instance
            BaseViewModel currentContext;
            try
            {
                currentContext = (BaseViewModel)Xamarin.Forms.Application.
                    Current.MainPage.Navigation.
                    NavigationStack.LastOrDefault().BindingContext;
            }
            catch (System.Exception)
            {
                base.OnBackPressed();
                return;
            }

            // check if the page has subscribed to 
            // the custom back button event
            if (currentContext?.CustomBackButtonAction != null)
            {
                currentContext?.CustomBackButtonAction.Invoke();
            }
            else
            {
                base.OnBackPressed();
            }
        }
    }
}