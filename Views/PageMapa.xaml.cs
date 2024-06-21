using PM2E12272.Models;

namespace PM2E12272.Views;

public partial class PageMapa : ContentPage
{
    string photo;

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
    private async System.Threading.Tasks.Task CheckGpsStatusAsync()
    {
        var locationStatus = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10)));

        if (locationStatus == null)
        {
            await DisplayAlert("El GPS no esta habilitado", "Por favor habilita el GPS", "OK");
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