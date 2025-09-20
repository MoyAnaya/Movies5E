using Movies5E.Models;
using System.Text;
using System.Text.Json;

namespace Movies5E;

public partial class AddMoviePage : ContentPage
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl = "https://1lh7j4n0-3000.usw3.devtunnels.ms/api"; 

    public AddMoviePage()
    {
        InitializeComponent(); 
        _httpClient = new HttpClient();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        
        string titulo = tituloEntry.Text?.Trim();
        string genero = generoPicker.SelectedItem?.ToString();
        string anoText = anoEntry.Text?.Trim();
        string urlImagen = UrlImagenEntry.Text?.Trim();

        
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

        
        await AddMovieAsync(titulo, genero, anoLanzamiento, urlImagen);
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Confirmar",
            "¿Cancelar sin guardar?", "Sí", "No");

        if (confirm)
        {
            await Navigation.PopAsync();
        }
    }

    private async Task AddMovieAsync(string titulo, string genero, int anoLanzamiento, string urlImagen)
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

            var response = await _httpClient.PostAsync($"{_baseUrl}/movies", content);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Éxito", "Película agregada correctamente", "OK");
                await Navigation.PopAsync(); 
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Error", $"No se pudo agregar: {error}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error de conexión: {ex.Message}", "OK");
        }
    }
}
