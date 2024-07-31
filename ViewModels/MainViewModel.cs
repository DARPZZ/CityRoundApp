using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vamdrup_rundt.Services;

namespace Vamdrup_rundt.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        [ObservableProperty]
        public string test;
        LocationService locationService = new LocationService();
        HashSet<string> streetNames = new HashSet<string>();

        public MainViewModel()
        {
            locationService.LocationUpdated += OnLocationUpdated; 
            locationService.OnStartListening();
        }

        private void OnLocationUpdated(object sender, EventArgs e)
        {
            ExtractStreetNames();
            print();
        }

        public void ExtractStreetNames()
        {
            
            foreach (var item in locationService.currentLocation)
            {
                streetNames.Add(item.FeatureName);
            }
        }

        public void print()
        {

            for (int i = 0; i < streetNames.Count; i++)
            {
                Debug.WriteLine(streetNames.ToList()[i]);
            }
        }
    }
}