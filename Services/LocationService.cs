using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Vamdrup_rundt.Services
{
    public class LocationService
    {
        private CancellationTokenSource _cancelTokenSource;
        private bool _isCheckingLocation;
        private bool _isListening;
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public HashSet<LocationModel> currentLocation { get; set; } = new HashSet<LocationModel>();
        public event EventHandler LocationUpdated;

        public async Task GetCurrentLocation()
        {
            Debug.WriteLine("Calling GetCurrentLocation");
            try
            {
                _isCheckingLocation = true;

                var request = new GeolocationRequest(GeolocationAccuracy.Best);

                _cancelTokenSource = new CancellationTokenSource();

                var location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

                if (location != null)
                {
                    Longitude = location.Longitude;
                    Latitude = location.Latitude;

                    Debug.WriteLine($"Location obtained: Longitude={Longitude}, Latitude={Latitude}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Can't get current location " + ex);
            }
            finally
            {
                _isCheckingLocation = false;
            }
        }

        public void CancelRequest()
        {
            if (_isCheckingLocation && _cancelTokenSource != null && !_cancelTokenSource.IsCancellationRequested)
                _cancelTokenSource.Cancel();
        }

        public async Task<string> GetGeocodeReverseData(double latitude, double longitude)
        {
            try
            {
                Debug.WriteLine("lat"+latitude + "Long "+ longitude  + "    ko");
                var placemarks = await Geocoding.Default.GetPlacemarksAsync(latitude, longitude);
                var placemark = placemarks?.FirstOrDefault();

                if (placemark != null)
                {
                    var locationModel = new LocationModel(
                        placemark.PostalCode.ToString(),
                        placemark.CountryCode,
                        placemark.Thoroughfare
                    );

                    currentLocation.Add(locationModel);
                    LocationUpdated?.Invoke(this, EventArgs.Empty);
                    
                    // Return a formatted string or use another way to return the data
                    return $"{placemark.Locality}";
                }

                return "No placemarks found.";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetGeocodeReverseData Exception: {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }

        public async void OnStartListening()
        {
            Debug.WriteLine("Attempting to start listening for location updates");
            try
            {
                Geolocation.LocationChanged += Geolocation_LocationChanged;
                var request = new GeolocationListeningRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
                var success = await Geolocation.StartListeningForegroundAsync(request);

                if (success)
                {
                    Debug.WriteLine("Listening for location updates started successfully.");
                    _isListening = true;
                }
                else
                {
                    _isListening = false;
                    Debug.WriteLine("Couldn't start listening for location updates.");
                }
            }
            catch (Exception ex)
            {
                _isListening = false;
                Debug.WriteLine($"OnStartListening Exception: {ex}");
            }
        }

        private async void Geolocation_LocationChanged(object sender, GeolocationLocationChangedEventArgs e)
        {
            var location = e.Location;
            Longitude = location.Longitude;
            Latitude = location.Latitude;
  
            Debug.WriteLine($"Location changed: Latitude={Latitude}, Longitude={Longitude}");
            await GetGeocodeReverseData(location.Latitude, location.Longitude);
        }
    }
}
