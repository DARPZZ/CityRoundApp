    using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Vamdrup_rundt.Models;
using Vamdrup_rundt.Services;

namespace Vamdrup_rundt.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        private int Count = 0;
        string emailLogin;
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
        LocationService locationService;
        VisitedStreetsDataService visitedStreetsDataService = new VisitedStreetsDataService();
        List<VisitedStreetsModel> streets = new List<VisitedStreetsModel>();
        private List<Pin> currentPins;
        
        public MainViewModel(LocationService locationService)
        {
            
            GetUserEmailFromLogin();
            StartStopTripText = "Start";
            IsTripActive = false;
            currentPins = new List<Pin>();
            this.locationService = locationService;
            this.locationService.LocationUpdated -= OnLocationUpdated;
            this.locationService.LocationUpdated += OnLocationUpdated;


        }
        private async void GetUserEmailFromLogin()
        {
            emailLogin = await SecureStorage.Default.GetAsync("email");
        }


        private async void OnLocationUpdated(object sender, EventArgs e)
        {
            var longg = locationService.Longitude;
            var lat = locationService.Latitude;

            ExtractStreetNames();
            Debug.WriteLine($"📍 OnLocationUpdated triggered: Lat={Lat}, Long={Longg}");
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                Longg = longg;
                Lat = lat;
                await PublishPinsUpdate();
            });
        }

        public void ExtractStreetNames()
        {
            foreach (var item in locationService.currentLocation)
            {
                if (item.FeatureName != null)
                {
                    var vs = new VisitedStreetsModel
                    {
                        StreetName = item.FeatureName,
                        Postnummer = int.Parse(item.PostalCode),
                    };
                    
                    
                    bool alreadyExists = streets.Any(s => s.StreetName == vs.StreetName && s.Postnummer == vs.Postnummer);

                    if (!alreadyExists)
                    {
                        streets.Add(vs);
                    }
                }
            }
        }


        public void Print()
        {
            for (int i = 0; i < streets.Count; i++)
            {
                Debug.WriteLine(streets.ToList()[i]);
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
               await publishstreetAsync();
            }
            else
            {
                IsTripActive = true;
                StartStopTripText = "Stop";
                locationService.OnStartListening();
            }
        }


        private async Task PublishPinsUpdate()
        {
            try
            {
                Count++;

                var pins = streets.Select(street => new Pin
                {
                    Label = street.StreetName,
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
        public async Task publishstreetAsync()
        {
            try
            {
                foreach (var item in streets)
                {
                    var vsstreet = new VisitedStreetsModel
                    {
                        email = emailLogin,
                        StreetName = item.StreetName,
                        Postnummer = item.Postnummer,
                    };
                    Debug.WriteLine(vsstreet.StreetName);
                    bool isCorrectCredentials = await visitedStreetsDataService.PostVisitedStreet(vsstreet);
                    if (isCorrectCredentials)
                    {
                        Debug.WriteLine("OK");
                    }else
                    {
                        Debug.WriteLine("The street is already loaded in");
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }


     
    }
}
