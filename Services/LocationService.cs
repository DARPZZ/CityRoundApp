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
        private double longitude;
        private double latitude;
        public HashSet<LocationModel> currentLocation { get; set; } = new HashSet<LocationModel>();
        public event EventHandler LocationUpdated; 

        private async Task<string> GetGeocodeReverseData(double latitude, double longitude)
        {
            try
            {
                var placemarks = await Geocoding.Default.GetPlacemarksAsync(latitude, longitude);
                var placemark = placemarks?.FirstOrDefault();

                if (placemark != null)
                {
                    LocationModel locationModel = new LocationModel(
                        placemark.PostalCode.ToString(),
                        placemark.CountryCode,
                        placemark.Thoroughfare
                    );

                    currentLocation.Add(locationModel);
                    LocationUpdated?.Invoke(this, EventArgs.Empty);

                    return $"AdminArea: {placemark.AdminArea}\nCountryCode: {placemark.CountryCode}\nCountryName: {placemark.CountryName}\nFeatureName: {placemark.FeatureName}\nLocality: {placemark.Locality}\nPostalCode: {placemark.PostalCode}\nSubAdminArea: {placemark.SubAdminArea}\nSubLocality: {placemark.SubLocality}\nSubThoroughfare: {placemark.SubThoroughfare}\nThoroughfare: {placemark.Thoroughfare}\n";
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
            try
            {
                Geolocation.LocationChanged += Geolocation_LocationChanged;
                var request = new GeolocationListeningRequest(GeolocationAccuracy.Best);
                var success = await Geolocation.StartListeningForegroundAsync(request);

                if (success)
                {
                    _isListening = true;
                }
                else
                {
                    _isListening = false;
                    Debug.WriteLine("Couldn't start listening");
                }
            }
            catch (Exception ex)
            {
                _isListening = false;
                Debug.WriteLine($"OnStartListening Exception: {ex}");
            }
        }

        void Geolocation_LocationChanged(object sender, GeolocationLocationChangedEventArgs e)
        {
            var location = e.Location;
            GetGeocodeReverseData(location.Latitude, location.Longitude);
        }
    }
}