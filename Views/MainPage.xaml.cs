using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace PM2E12272.Views;

public partial class MainPage : ContentPage
{
    private const string GoogleMapsApiKey = "AIzaSyARoSxWhYLNwTLRy1RuWBZAc8fscCjwGes";
    Controllers.UbicacionControllers controller;
    FileResult photo;

    public MainPage()
    {
        InitializeComponent();
        CheckGpsStatusAsync();
        controller = new Controllers.UbicacionControllers();
    }

    public MainPage(Controllers.UbicacionControllers dbPath)
    {
        InitializeComponent();
        controller = dbPath;
    }

    public string? GetImg64()
    {
        if (photo != null)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Stream stream = photo.OpenReadAsync().Result;
                stream.CopyTo(ms);
                byte[] data = ms.ToArray();

                String Base64 = Convert.ToBase64String(data);

                return Base64;
            }
        }
        return null;
    }

    private async System.Threading.Tasks.Task CheckGpsStatusAsync()
    {
        var locationStatus = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10)));

        if (locationStatus == null)
        {
            await DisplayAlert("El GPS no esta habilitado", "Por favor habilita el GPS", "OK");
        }
    }

    private async void btnAgregar_Clicked(object sender, EventArgs e)
    {
        string foto = GetImg64();
        string latitud = txtLatitud.Text;
        string longitud = txtLongitud.Text;
        string descripcion = txtDescripcion.Text;

        if (string.IsNullOrEmpty(foto))
        {
            await DisplayAlert("Error", "Por favor ingrese una foto", "OK");
            return;
        }
        else if (string.IsNullOrEmpty(latitud))
        {
            await DisplayAlert("Error", "Espacio de latitud vacio, por favor de click en el boton Buscar Ubicacion", "OK");
            return;
        }
        else if (string.IsNullOrEmpty(longitud))
        {
            await DisplayAlert("Error", "Espacio de longitud vacio, por favor de click en el boton Buscar Ubicacion", "OK");
            return;
        }
        else if (string.IsNullOrEmpty(descripcion))
        {
            await DisplayAlert("Error", "Por favor ingrese una ubicacion", "OK");
            return;
        }

        var ubi = new Models.Ubicacion
        {
            Latitud = txtLatitud.Text,
            Longitud = txtLongitud.Text,
            Descripcion = txtDescripcion.Text,
            Foto = GetImg64()
        };

        try
        {
            if (controller != null)
            {
                if (await controller.storeUbicacion(ubi) > 0)
                {
                    await DisplayAlert("Aviso", "Registro Ingresado con Exito!", "OK");
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Error", "Ocurrio un Error", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrio un Error: {ex.Message}", "OK");
        }

        imgFoto.Source = "logoubi.png";
        txtLatitud.Text = string.Empty;
        txtLongitud.Text = string.Empty;
        txtDescripcion.Text = string.Empty;
    }

    private async void btnfoto_Clicked(object sender, EventArgs e)
    {
        photo = await MediaPicker.CapturePhotoAsync();

        if (photo != null)
        {
            string photoPath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
            using Stream sourcephoto = await photo.OpenReadAsync();
            using FileStream streamlocal = File.OpenWrite(photoPath);

            imgFoto.Source = ImageSource.FromStream(() => photo.OpenReadAsync().Result); 

            await sourcephoto.CopyToAsync(streamlocal); 
        }
    }

    private void btnRegresar_Clicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    private void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new PageList());
    }

    private async void btnBuscar_Clicked(object sender, EventArgs e)
    {
        string descripcion = txtDescripcion.Text;
        if (string.IsNullOrEmpty(descripcion))
        {
            await DisplayAlert("Error", "Por favor ingrese la ubicacion", "OK");
            return;
        }

        string requestUri = $"https://maps.googleapis.com/maps/api/geocode/json?address={descripcion}&key={GoogleMapsApiKey}";

        using (HttpClient client = new HttpClient())
        {
            string response = await client.GetStringAsync(requestUri);
            var json = JObject.Parse(response);

            if (json["status"].ToString() == "OK")
            {
                var location = json["results"][0]["geometry"]["location"];
                double latitude = (double)location["lat"];
                double longitude = (double)location["lng"];

                txtLatitud.Text = latitude.ToString();
                txtLongitud.Text = longitude.ToString();
            }
            else
            {
                await DisplayAlert("Error", "Ingrese una ubicacion existente", "OK");
            }
        }
    }
}