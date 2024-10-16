using AppLanches.Models;
using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches.Pages;

public partial class HomePage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;

    public HomePage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        LblNomeUsuario.Text = "Hi, " + Preferences.Get("usuarionome", string.Empty);
        _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        _validator = validator;
        Title = AppConfig.tituloHomePage;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetListCategorias();
        await GetListBestSellers();
        await GetListMostPopulars();
    }
   
    private async Task<IEnumerable<Produto>> GetListMostPopulars()
    {
        try
        {
            var (produtos, errorMessage) = await _apiService.GetProdutos("popular", string.Empty);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Produto>();
            }
            if (produtos == null)
            {
                await DisplayAlert("Error", errorMessage ?? "It was possible to obtain the products.", "OK");
                return Enumerable.Empty<Produto>();
            }

            CvPopulares.ItemsSource = produtos;
            return produtos;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Something went wrong:{ex.Message}", "OK");
            return Enumerable.Empty<Produto>();
        }

    }

    private async Task <IEnumerable<Produto>> GetListBestSellers()
    {
        try
        {
            var (produtos, errorMessage) = await _apiService.GetProdutos("maisvendido", string.Empty);
           
            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Produto>();
            }
            if (produtos == null)
            {
                await DisplayAlert("Error", errorMessage ?? "It was possible to obtain the products.", "OK");
                return Enumerable.Empty<Produto>();
            }

            CvMaisVendidos.ItemsSource = produtos;
            return produtos;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Something went wrong:{ex.Message}", "OK");
            return Enumerable.Empty<Produto>();
        }
    }

    private async Task<IEnumerable<Categoria>> GetListCategorias()
    {
        try
        {
            var (categorias, errorMessage) = await _apiService.GetCategorias();
            if (errorMessage == "Unauthorized")
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Categoria>();
            }
            if ( categorias == null)
            {
                await DisplayAlert("Error", errorMessage ?? "It was possible to obtain the categories.", "OK");
                return Enumerable.Empty<Categoria>();
            }

            CvCategorias.ItemsSource = categorias;
            return categorias;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Something went wrong:{ex.Message}", "OK");
            return Enumerable.Empty<Categoria>();
        }
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushModalAsync(new LoginPage(_apiService, _validator));
    }

    private void CvCategorias_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as Categoria;

        if (currentSelection is null)
        {
            return;
        }

        Navigation.PushAsync(new ListaProdutosPage(
            currentSelection.Id,
            currentSelection.Nome!,
            _apiService,
            _validator));

        ((CollectionView)sender).SelectedItem = null;

    }
}