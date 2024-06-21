using PM2E12272.Models;

namespace PM2E12272.Views;

public partial class PageMapa : ContentPage
{
    
    string photo;
    Controllers.UbicacionControllers controller;
    List<Models.Ubicacion> ubicacion;

    public PageMapa(Ubicacion tabla)
	{
		InitializeComponent();
        CheckGpsStatusAsync();
        photo = tabla.Foto;
        double lati;
        double longi;
        double.TryParse(tabla.Latitud, out lati);
        double.TryParse(tabla.Longitud, out longi);

        LoadGoogleMaps(lati, longi);
    }

    private void LoadGoogleMaps(double latitude, double longitude)
    {
        var htmlSource = new HtmlWebViewSource
        {
            Html = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <title>Google Maps</title>
                </head>
                <body>
                    <iframe width='100%' height='100%' frameborder='0' style='border:0'
                        src='https://www.google.com/maps/embed/v1/view?key=AIzaSyARoSxWhYLNwTLRy1RuWBZAc8fscCjwGes&center={latitude},{longitude}&zoom=15' allowfullscreen>
                    </iframe>
                </body>
                </html>"
        };

        webView.Source = htmlSource;
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
            await DisplayAlert("Alerta", "La aplicación necesita permisos de ubicación para funcionar.", "OK");
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
            await DisplayAlert("GPS no activado", "El GPS no está activado. Por favor, habilítelo en la configuración.", "OK");
        }
    }


    private void btnRegresar_Clicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    private async void btnCompartir_Clicked(object sender, EventArgs e)
    {
        byte[] imageBytes = Convert.FromBase64String(photo);
        string tempFilePath = Path.Combine(FileSystem.CacheDirectory, "tempImage.png");
        File.WriteAllBytes(tempFilePath, imageBytes);

        await ShareFileAsync(tempFilePath);
    }

    private async Task ShareFileAsync(string filePath)
    {
        await Share.RequestAsync(new ShareFileRequest
        {
            Title = "Compartir Imagen",
            File = new ShareFile(filePath)
        });
    }
}