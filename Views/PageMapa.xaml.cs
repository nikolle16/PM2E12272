using PM2E12272.Models;
using System.Runtime.InteropServices;

namespace PM2E12272.Views;

public partial class PageMapa : ContentPage
{
    private double lati;
    private double longi;
    Controllers.UbicacionControllers controller;
    List<Models.Ubicacion> ubicacion;

    public PageMapa(Ubicacion tabla)
	{
		InitializeComponent();
        CheckGpsStatusAsync();
        double.TryParse(tabla.Latitud, out lati);
        double.TryParse(tabla.Longitud, out longi);
    }

    private async Task CheckGpsStatusAsync()
    {
        var locationStatus = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

        if (locationStatus != PermissionStatus.Granted)
        {
            locationStatus = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }

        if (locationStatus != PermissionStatus.Granted)
        {
            await DisplayAlert("Alerta", "La aplicaci�n necesita permisos de ubicaci�n para funcionar.", "OK");
            return;
        }

        var location = await Geolocation.GetLastKnownLocationAsync();

        if (location == null)
        {
            location = await Geolocation.GetLocationAsync(new GeolocationRequest
            {
                DesiredAccuracy = GeolocationAccuracy.Medium,
                Timeout = TimeSpan.FromSeconds(30)
            });
        }

        if (location == null)
        {
            await DisplayAlert("GPS no activado", "El GPS no est� activado. Por favor, habil�telo en la configuraci�n.", "OK");
        }
    }

    private void btnRegresar_Clicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    private void btnCompartir_Clicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }
}