using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Microsoft.Maui.ApplicationModel;
using System.Diagnostics;

namespace Vamdrup_rundt.Services
{
    [Service(Name = "com.companyname.vamdrup_rundt.AndroidLocationService", ForegroundServiceType = Android.Content.PM.ForegroundService.TypeLocation)]
    public class AndroidLocationService : Service
    {
        public const string ActionStart = "Vamdrup_rundt.Action.Start";
        public const string ActionStop = "Vamdrup_rundt.Action.Stop";
        public const int ServiceRunningNotificationId = 10001;

        private LocationService _sharedLocationService;

        public override IBinder OnBind(Intent intent) => null;

        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            
            _sharedLocationService = IPlatformApplication.Current.Services.GetService<LocationService>();

            if (intent.Action == ActionStart)
            {
                StartForegroundService();
                StartLocationUpdates();
            }
            else if (intent.Action == ActionStop)
            {
                StopLocationUpdates();
                StopForeground(true);
                StopSelf();
            }

            return StartCommandResult.Sticky;
        }
        private string RandomContextText()
        {
            List<string> text = new List<string>
            {
                "Vi følger med, men på den søde måde 😇",
                "Bare rolig, vi ved kun hvor du er – ikke hvad du laver 😏",
                "Vi stalker dig... men med samtykke!",
                "Din skygge har fået konkurrence – vi følger dine skridt!",
                "Vi går med dig. Usynligt. Hele tiden.",
                "GPS’en har tændt for forfølgelsen",
                "Vi samler dine skridt som Pokémon – gotta track 'em all!",
                "Mission: Lokation – vi har dig på radaren",
                "Din lokation er live... 007-style 🕵️",
                "Vi er som en spionfilm – minus eksplosionerne"
            };
            int arrayLenght = text.Count;
            Random rnd = new Random();
            var randomNumber = rnd.Next(arrayLenght);
            return text[randomNumber];
        }

        private void StartForegroundService()
        {
            var channelId = "LocationServiceChannel";
            var channelName = "Location Service";
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(channelId, channelName, NotificationImportance.Default);
                notificationManager.CreateNotificationChannel(channel);
            }

            
            var notification = new Notification.Builder(this, channelId)
                .SetContentTitle("Vamdrup Rundt")
                .SetContentText(RandomContextText())
                .SetSmallIcon(Resource.Mipmap.appicon)
                .SetOngoing(true)
                .Build();

            
            StartForeground(ServiceRunningNotificationId, notification, Android.Content.PM.ForegroundService.TypeLocation);
        }

        private void StartLocationUpdates()
        {
            try
            {

                Geolocation.LocationChanged += OnLocationChanged;
                var request = new GeolocationListeningRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(5));


                Task.Run(async () => await Geolocation.StartListeningForegroundAsync(request));

                Console.WriteLine("Android Service: Started listening for location changes.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Android Service: Error starting location updates: {ex.Message}");
            }
        }

        private void StopLocationUpdates()
        {
            Geolocation.LocationChanged -= OnLocationChanged;
            Geolocation.StopListeningForeground();
            Console.WriteLine("Android Service: Stopped listening for location changes.");
        }

  
        void OnLocationChanged(object sender, GeolocationLocationChangedEventArgs e)
        {
            var location = e.Location;
            if (location != null && _sharedLocationService != null)
            {
                
                _sharedLocationService.HandleLocationUpdateFromPlatform(location);

                Console.WriteLine($"Android Service: Location Changed: {location.Latitude}, {location.Longitude}");
            }
        }

        public override void OnDestroy()
        {
            StopLocationUpdates();
            base.OnDestroy();
        }
    }
}
