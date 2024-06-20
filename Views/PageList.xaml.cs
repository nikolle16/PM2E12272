using System.Collections.ObjectModel;
using 

namespace PM2E12272.Views;

public partial class PageList : ContentPage
{
    private Controllers.AutorController AutorController;
    private List<Models.Autor> autores;
    Models.Autor selectedAuthor;
    private AutorController controller;
    public ObservableCollection<Autor> Autores { get; set; }
    public Command<Autor> UpdateCommand { get; }
    public Command<Autor> DeleteCommand { get; }

    public verAutores()
    {
        InitializeComponent();
        AutorController = new Controllers.AutorController();
        controller = new AutorController();
        Autores = new ObservableCollection<Autor>();
        BindingContext = this;
    }

    //Metodo que permite mostrar la lista mientras la pagina se esta mostrando o cargando
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Obtiene la lista de personas de la base de datos
        autores = await AutorController.getListAutor();

        // Coloca la lista en el collection view
        collectionView.ItemsSource = autores;
    }

    private void searchBar_SearchButtonPressed(object sender, EventArgs e)
    {
        BuscarAutores(searchBar.Text);
    }

    //Funcion para realizar una busqueda en la lista o base de datos
    private void BuscarAutores(string query)
    {

        //Usa LINQ (Language-Integrated Query) (es una característica en el framework .NET que proporciona una sintaxis estandarizada
        //y declarativa para consultar y manipular datos de diferentes tipos de fuentes, como colecciones,
        //bases de datos, XML, entre otros.) en una expresion tipo lambda para filtrar la informacion de la base 
        //de datos y mostrar los resultados basados en la busqueda.

        var results = autores
            .Where(author => author.Nombres?.ToLowerInvariant().Contains(query.ToLowerInvariant()) == true ||
                             author.Nacionalidad?.ToLowerInvariant().Contains(query.ToLowerInvariant()) == true)
            .ToList();

        collectionView.ItemsSource = new List<Models.Autor>(results);
    }

    private void searchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(e.NewTextValue))
        {
            collectionView.ItemsSource = autores;
        }
    }

    private void btnRegresar_Clicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    private void collectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        selectedAuthor = e.CurrentSelection.FirstOrDefault() as Models.Autor;
    }

    private async void ActualizarAutor_Clicked(object sender, EventArgs e)
    {
        if (selectedAuthor != null)
        {
            await Navigation.PushAsync(new actuAutor(selectedAuthor.Id));
        }
        else
        {
            await DisplayAlert("Error", "Seleccione un autor primero", "OK");
        }
    }

    private async void EliminarAutor_Clicked(object sender, EventArgs e)
    {
        var result = await DisplayAlert("Confirmar", "¿Está seguro que desea eliminar este autor?", "Sí", "No");

        if (selectedAuthor != null)
        {
            if (result)
            {
                await controller.deleteAutor(selectedAuthor.Id);
                Autores.Remove(selectedAuthor);

                Navigation.PopAsync();
            }
            else
            {
                return;
            }
        }
        else
        {
            await DisplayAlert("Error", "Seleccione un autor primero", "OK");
        }
    }
}