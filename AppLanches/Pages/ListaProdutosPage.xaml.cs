using AppLanches.Models;
using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches.Pages;

public partial class ListaProdutosPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;
    private int _categoriaId;


    public ListaProdutosPage(int categoriaId, string categoriaNome, ApiService apiService, IValidator validator )
	{
		InitializeComponent();
        _categoriaId = categoriaId;
        _apiService = apiService;
        _validator = validator;
        Title = categoriaNome ?? "Products";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetListProdutos(_categoriaId);
    }

    private async Task<IEnumerable<Produto>> GetListProdutos(int categoriaId)
    {
        try
        {
            var (produtos, errorMessage) = await _apiService.GetProdutos("categoria", categoriaId.ToString());

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Produto>();
            }

            if (produtos is null)
            {
                await DisplayAlert("Error", errorMessage ?? "It was not possible to obtain the products.", "OK");
                return Enumerable.Empty<Produto>();
            }

            CvProdutos.ItemsSource = produtos;
            return produtos;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Something went wrong: {ex.Message}", "OK");
            return Enumerable.Empty<Produto>();
        }
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }
}