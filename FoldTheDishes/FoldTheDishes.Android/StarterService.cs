using Android.App;
using Android.Content;
using Android.OS;

namespace FoldTheDishes.Droid
{
    [Service(Enabled = true)]
    public class StarterService : Service
    {
        public override void OnCreate()
        {
            base.OnCreate();
            System.Diagnostics.Debug.WriteLine("Sticky Service - Created");
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            return StartCommandResult.Sticky;
        }

        public override IBinder OnBind(Intent intent)
        {
            System.Diagnostics.Debug.WriteLine("Sticky Service - Binded");
            return null;
        }

        public override void OnDestroy()
        {
            System.Diagnostics.Debug.WriteLine("Sticky Service - Destroyed");
            base.OnDestroy();
        }
    }
}