using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Vamdrup_rundt.Services;

namespace Vamdrup_rundt.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        private int Count = 0;
        public string PinLabel { get; set; }
        public string Address { get; set; }
        public Location MyLocation { get; set; }
        private bool IsTripActive;
        [ObservableProperty]
        public string test;
        [ObservableProperty]
        public string ct;
        [ObservableProperty]
        public double longg;
        [ObservableProperty]
        public double lat;
        [ObservableProperty]
        private string startStopTripText;

        [ObservableProperty]
        public string streetsText;

        private readonly LocationService locationService = new LocationService();
        private readonly HashSet<string> streetNames;
        private List<Pin> currentPins;
        
        public MainViewModel()
        {
            locationService = new LocationService();
            streetNames = new HashSet<string>();
            StartStopTripText = "Start";
            IsTripActive = false;
            currentPins = new List<Pin>();
          
            locationService.LocationUpdated += OnLocationUpdated;


        }

        private async void OnLocationUpdated(object sender, EventArgs e)
        {
            Longg = locationService.Longitude;
            Lat = locationService.Latitude;
            
            ExtractStreetNames();
            PublishPinsUpdate();
        }

        public void ExtractStreetNames()
        {
            foreach (var item in locationService.currentLocation)
            {
                if(item.FeatureName != null)
                {
                    streetNames.Add(item.FeatureName);

                    StreetsText = string.Join(", ", streetNames);
                }
               
            }
        }

        public void Print()
        {
            for (int i = 0; i < streetNames.Count; i++)
            {
                Debug.WriteLine(streetNames.ToList()[i]);
            }
        }

        [RelayCommand]
        private async Task OnStartStopTripClicked()
        {
            if (IsTripActive)
            {
               locationService.OnStopListening();
               IsTripActive= false;
               StartStopTripText = "Start";
            }
            else
            {
                IsTripActive = true;
                StartStopTripText = "Stop";
                locationService.OnStartListening();
            }
        }


        private void PublishPinsUpdate()
        {
            try
            {
                Count++;

                var pins = streetNames.Select(street => new Pin
                {
                    Label = street,
                    Type = PinType.Place,
                    Location = new Location(locationService.Latitude, locationService.Longitude)
                }).ToList();


                EventAggregator.Instance.Publish(new MapPinsUpdatedEvent { Pins = pins });

            }
            catch (Exception ex)
            {
                Debug.WriteLine("No street name found");
            }
        }
     
    }
}
