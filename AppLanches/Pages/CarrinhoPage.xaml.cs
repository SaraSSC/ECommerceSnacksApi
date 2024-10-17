using AppLanches.Models;
using AppLanches.Services;
using AppLanches.Validations;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace AppLanches.Pages;

public partial class CarrinhoPage : ContentPage
{
	private readonly ApiService _apiService;
	private readonly IValidator _validator;
    private ObservableCollection<CarrinhoCompraItem> ItensCarrinhoCompra = new ObservableCollection<CarrinhoCompraItem>();
    private bool _loginPageDisplayed = false;
    private bool _isNavigatingEmptyCartPage = false;
    public CarrinhoPage(ApiService apiService, IValidator validator)
	{
		InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetItensCarrinhoCompra();
    }

    private async Task<IEnumerable<CarrinhoCompraItem>> GetItensCarrinhoCompra()
	{
		try
		{
			var usuarioId = Preferences.Get("usuarioid", 0);
			var (itensCarrinhoCompra, errorMessage) = await _apiService.GetItensCarrinhoCompra(usuarioId);

			if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
			{
				await DisplayLoginPage();
				return Enumerable.Empty<CarrinhoCompraItem>();
			}

			if (itensCarrinhoCompra is null)
			{
				await DisplayAlert("Error", errorMessage ?? "It was not possible to obtain the shopping cart items.", "OK");
				return Enumerable.Empty<CarrinhoCompraItem>();
			}

			ItensCarrinhoCompra.Clear();
			foreach (var item in itensCarrinhoCompra)
			{
				ItensCarrinhoCompra.Add(item);
			}

			CvCarrinho.ItemsSource = ItensCarrinhoCompra;
			AtualizaPrecoTotal();
			return itensCarrinhoCompra;
		}
		catch (Exception ex)
		{

			await DisplayAlert("Erro", $"An error was occured: {ex.Message}", "OK");
            return Enumerable.Empty<CarrinhoCompraItem>();
        }
    }

	private void AtualizaPrecoTotal()
    {
        try
        {
            var precoTotal = ItensCarrinhoCompra.Sum(item => item.Preco * item.Quantidade);
            LblPrecoTotal.Text = precoTotal.ToString();
        }
        catch (Exception ex)
        {
            DisplayAlert("Erro", $"An error occured while trying to update the final price: {ex.Message}", "OK");
        }
    }

   

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;

        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    private void BtnEditaEndereco_Clicked(object sender, EventArgs e)
    {

    }

    private void BtnDeletar_Clicked(object sender, EventArgs e)
    {

    }

    private void BtnIncrementar_Clicked(object sender, EventArgs e)
    {

    }

    private void BtnDecrementar_Clicked(object sender, EventArgs e)
    {

    }

    private void TapConfirmarPedido_Tapped(object sender, TappedEventArgs e)
    {

    }
}