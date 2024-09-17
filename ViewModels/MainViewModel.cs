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

        private readonly LocationService locationService = new LocationService();
        VisitedStreetsDataService visitedStreetsDataService = new VisitedStreetsDataService();
        List<VisitedStreetsModel> streets = new List<VisitedStreetsModel>();
        private List<Pin> currentPins;
        
        public MainViewModel()
        {
            
            GetUserEmailFromLogin();
            locationService = new LocationService();
            StartStopTripText = "Start";
            IsTripActive = false;
            currentPins = new List<Pin>();
          
            locationService.LocationUpdated += OnLocationUpdated;


        }
        private async void GetUserEmailFromLogin()
        {
            emailLogin = await SecureStorage.Default.GetAsync("email");
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
                var vs = new VisitedStreetsModel {
                    StreetName = item.FeatureName,
                    Postnummer = int.Parse(item.PostalCode),

                };
              
                if(item.FeatureName != null)
                {
                    streets.Add(vs);
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
                publishstreetAsync();
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
