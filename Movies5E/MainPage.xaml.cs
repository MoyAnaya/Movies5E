using Movies5E.Models;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text;

namespace Movies5E;

public partial class MainPage : ContentPage
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl = "https://1lh7j4n0-3000.usw3.devtunnels.ms/api";
    private ObservableCollection<Movie> _movies;

    public MainPage()
    {
        InitializeComponent();
        _httpClient = new HttpClient();
        _movies = new ObservableCollection<Movie>();
        lista_peliculas.ItemsSource = _movies;

        // Cargar películas al iniciar
        _ = LoadMoviesAsync();
    }

    private async void OnAddMovieClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddMoviePage());
    }

    private async void OnEditMovieClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Movie movie)
        {
            await Navigation.PushAsync(new EditMoviePage(movie));
        }
    }

    private async void OnDeleteMovieClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Movie movie)
        {
            bool confirm = await DisplayAlert("Confirmar", $"¿Eliminar '{movie.Titulo}'?", "Sí", "No");
            if (confirm)
            {
                await DeleteMovieAsync(movie.Id);
            }
        }
    }

    private async Task LoadMoviesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/movies");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var movies = JsonSerializer.Deserialize<List<Movie>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                _movies.Clear();
                foreach (var movie in movies ?? new List<Movie>())
                {
                    _movies.Add(movie);
                }
            }
            else
            {
                await DisplayAlert("Error", "No se pudieron cargar las películas", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error de conexión: {ex.Message}", "OK");
        }
    }
    private async Task DeleteMovieAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/movies/{id}");
            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Éxito", "Película eliminada correctamente", "OK");
                await LoadMoviesAsync();
            }
            else
            {
                await DisplayAlert("Error", "No se pudo eliminar la película", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error: {ex.Message}", "OK");
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadMoviesAsync(); // 🔄 recarga las películas cada vez que vuelves
    }
}
