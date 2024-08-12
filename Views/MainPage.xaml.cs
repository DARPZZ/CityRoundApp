using System.Net.NetworkInformation;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Vamdrup_rundt.Services;

namespace Vamdrup_rundt.Views;

public partial class MainPage : ContentPage
{

    LocationService _locationService = new LocationService();
	public MainPage(MainViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
        EventAggregator.Instance.Subscribe<MapPinsUpdatedEvent>(OnMapPinsUpdated);
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
      
       CreateMapSpan();
       
    }
    private void OnMapPinsUpdated(MapPinsUpdatedEvent mapPinsUpdatedEvent)
    {
        CreateMapSpan();
        foreach (var pin in mapPinsUpdatedEvent.Pins)
        {
            myMap.Pins.Add(pin);
        }
    }
    private async void  CreateMapSpan()
    {
        await _locationService.GetCurrentLocation();
        var latitude = _locationService.Latitude;
        var longitude = _locationService.Longitude;
        var location = new Location(latitude, longitude);
        var mapSpan = new MapSpan(location, 0.004, 0.004);
        myMap.MoveToRegion(mapSpan);
    }
   


}
