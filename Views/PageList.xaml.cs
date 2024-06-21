using System.Collections.ObjectModel;
using PM2E12272.Models;
using PM2E12272.Controllers;
using Microsoft.Extensions.Primitives;
using System.Runtime.InteropServices;

namespace PM2E12272.Views;

public partial class PageList : ContentPage
{
    private Controllers.UbicacionControllers UbicacionControllers;
    private List<Models.Ubicacion> autores;
    Models.Ubicacion selectedUbicacion;
    private UbicacionControllers controller;
    public ObservableCollection<Ubicacion> Ubicacion { get; set; }
    public Command<Ubicacion> UpdateCommand { get; }
    public Command<Ubicacion> DeleteCommand { get; }

    public PageList()
    {
        InitializeComponent();

        UbicacionControllers = new Controllers.UbicacionControllers();
        controller = new UbicacionControllers();
        Ubicacion = new ObservableCollection<Ubicacion>();
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        autores = await UbicacionControllers.getListUbicacion();

        collectionView.ItemsSource = autores;
    }

    private void btnRegresar_Clicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    private void collectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        selectedUbicacion = e.CurrentSelection.FirstOrDefault() as Models.Ubicacion;
    }

    private async void ActualizarUbicacion_Clicked(object sender, EventArgs e)
    {
        if (selectedUbicacion != null)
        {
            await Navigation.PushAsync(new actuUbicacion(selectedUbicacion.Id));
        }
        else
        {
            await DisplayAlert("Error", "Seleccione una ubicacion primero", "OK");
        }
    }

    private async void EliminarUbicacion_Clicked(object sender, EventArgs e)
    {
        var result = await DisplayAlert("Confirmar", "¿Está seguro que desea eliminar esta ubicacion?", "Sí", "No");

        if (selectedUbicacion != null)
        {
            if (result)
            {
                await controller.deleteUbicacion(selectedUbicacion.Id);
                Ubicacion.Remove(selectedUbicacion);

                var currentPage = this;
                await Navigation.PushAsync(new PageList());
                Navigation.RemovePage(currentPage);
            }
            else
            {
                return;
            }
        }
        else
        {
            await DisplayAlert("Error", "Seleccione una ubicacion primero", "OK");
        }
    }

    private async void VerMapa_Clicked(object sender, EventArgs e)
    {
        if (selectedUbicacion != null)
        {

            await Navigation.PushAsync(new PageMapa(selectedUbicacion));
        }
        else
        {
            await DisplayAlert("Error", "Seleccione una ubicacion primero", "OK");
        }
    }
}