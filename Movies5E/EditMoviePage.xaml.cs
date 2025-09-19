using Movies5E.Models;
using System.Text;
using System.Text.Json;

namespace Movies5E;

public partial class EditMoviePage : ContentPage
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl = "https://1lh7j4n0-3000.usw3.devtunnels.ms/api";
    private readonly Movie _movieToEdit;

    // ¡Constructor que recibe la película a editar!
    public EditMoviePage(Movie movie)
    {
        InitializeComponent(); // Ahora sí funciona porque tienes el XAML
        _httpClient = new HttpClient();
        _movieToEdit = movie;

        // Llenar campos con datos existentes
        LoadMovieData();
    }

    private void LoadMovieData()
    {
        tituloEntry.Text = _movieToEdit.Titulo;
        generoEntry.Text = _movieToEdit.Genero;
        anoEntry.Text = _movieToEdit.AnoLanzamiento.ToString();
        UrlImagenEntry.Text = _movieToEdit.UrlImagen ?? "";
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        // Obtener datos del formulario
        string titulo = tituloEntry.Text?.Trim();
        string genero = generoEntry.Text?.Trim();
        string anoText = anoEntry.Text?.Trim();
        string urlImagen = UrlImagenEntry.Text?.Trim();

        // Validaciones
        if (string.IsNullOrEmpty(titulo))
        {
            await DisplayAlert("Error", "El título es obligatorio", "OK");
            return;
        }

        if (string.IsNullOrEmpty(genero))
        {
            await DisplayAlert("Error", "El género es obligatorio", "OK");
            return;
        }

        if (!int.TryParse(anoText, out int anoLanzamiento) || anoLanzamiento < 1900)
        {
            await DisplayAlert("Error", "Ingresa un año válido", "OK");
            return;
        }

        // Actualizar película
        await UpdateMovieAsync(titulo, genero, anoLanzamiento, urlImagen);
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Confirmar",
            "¿Cancelar sin guardar los cambios?", "Sí", "No");

        if (confirm)
        {
            await Navigation.PopAsync();
        }
    }

    private async Task UpdateMovieAsync(string titulo, string genero, int anoLanzamiento, string urlImagen)
    {
        try
        {
            var movieData = new
            {
                titulo = titulo,
                genero = genero,
                anoLanzamiento = anoLanzamiento,
                urlImagen = string.IsNullOrEmpty(urlImagen) ? null : urlImagen
            };

            var json = JsonSerializer.Serialize(movieData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{_baseUrl}/movies/{_movieToEdit.Id}", content);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Éxito", "Película actualizada correctamente", "OK");
                await Navigation.PopAsync(); // Regresar a MainPage
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Error", $"No se pudo actualizar: {error}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error de conexión: {ex.Message}", "OK");
        }
    }
}