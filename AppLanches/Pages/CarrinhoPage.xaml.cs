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
    private bool _isNavigationToEmptyCartPage = false;
    public CarrinhoPage(ApiService apiService, IValidator validator)
	{
		InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        //await GetItensCarrinhoCompra();

        

        if (IsNavigationToEmptyCartPage()) return;

        bool hasItems = await GetItensCarrinhoCompra();

        if (hasItems)
        {
            ExibirEndereco();
        }
        else
        {
            await NavegarParaCarrinhoVazio();
        }
    }


    private void ExibirEndereco()
    {
        bool enderecoSalvo = Preferences.ContainsKey("endereco");

        if (enderecoSalvo)
        {
            string nome = Preferences.Get("nome", string.Empty);
            string endereco = Preferences.Get("endereco", string.Empty);
            string telefone = Preferences.Get("telefone", string.Empty);

            LblEndereco.Text = $"{nome}\n{endereco}\n{telefone}";
        }
        else
        {
            LblEndereco.Text = "Add your address";
        }
    }

    private async Task NavegarParaCarrinhoVazio()
    {
        LblEndereco.Text = string.Empty;
        _isNavigatingEmptyCartPage = true;
        await Navigation.PushAsync(new CarrinhoVazioPage(_apiService, _validator));
    }

    private bool IsNavigationToEmptyCartPage()
    {
        if (_isNavigatingEmptyCartPage)
        {
            _isNavigatingEmptyCartPage = false;
            return true;
        }
        return false;
    }

    private async Task<bool> GetItensCarrinhoCompra()
	{
		try
		{
			var usuarioId = Preferences.Get("usuarioid", 0);
			var (itensCarrinhoCompra, errorMessage) = await _apiService.GetItensCarrinhoCompra(usuarioId);

			if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
			{
				await DisplayLoginPage();
				return false;
            }

			if (itensCarrinhoCompra is null)
			{
				await DisplayAlert("Error", errorMessage ?? "It was not possible to obtain the shopping cart items.", "OK");
                return false;
			}

			ItensCarrinhoCompra.Clear();
			foreach (var item in itensCarrinhoCompra)
			{
				ItensCarrinhoCompra.Add(item);
			}

			CvCarrinho.ItemsSource = ItensCarrinhoCompra;
			AtualizaPrecoTotal();
			
            if (!ItensCarrinhoCompra.Any())
            {
                return false;
            }
            return true;
		}
		catch (Exception ex)
		{

			await DisplayAlert("Erro", $"An error was occured: {ex.Message}", "OK");
            return false;
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
        Navigation.PushAsync(new EnderecoPage());
    }

    private async void BtnDeletar_Clicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.BindingContext is CarrinhoCompraItem itemCarrinho)
        {
            bool resposta = await DisplayAlert("Delete item", "Do you want to delete this item from the cart?", "Yes", "No");

            if (resposta) 
            {
                ItensCarrinhoCompra.Remove(itemCarrinho);
                AtualizaPrecoTotal();
                await _apiService.AtualizaQuantidadeItemCarrinho(itemCarrinho.ProdutoId, "delete");//deletar
            }
        }
    }

    private async void BtnIncrementar_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is CarrinhoCompraItem itemCarrinho)
        {
            itemCarrinho.Quantidade++;
            AtualizaPrecoTotal();
            await _apiService.AtualizaQuantidadeItemCarrinho(itemCarrinho.ProdutoId, "increase"); //aumentar
        }
    }

    private async void BtnDecrementar_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is CarrinhoCompraItem itemCarrinho)
        {
            if (itemCarrinho.Quantidade is 1)
            {
                return;
            }
            else
            {
                itemCarrinho.Quantidade--;
                AtualizaPrecoTotal();
                await _apiService.AtualizaQuantidadeItemCarrinho(itemCarrinho.ProdutoId, "decrease"); //diminuir
            }
           
        }
    }

    private async void TapConfirmarPedido_Tapped(object sender, TappedEventArgs e)
    {
        if (ItensCarrinhoCompra is null || !ItensCarrinhoCompra.Any())
        {
            await DisplayAlert("Information", "There are no items in the cart or your order is already confirmed.", "OK");
            return;
        }

        var pedido = new Pedido
        {
            Endereco = LblEndereco.Text,
            UsuarioId = Preferences.Get("usuarioid", 0),
            ValorTotal = Convert.ToDecimal(LblPrecoTotal.Text),
        };

        var response = await _apiService.ConfirmarPedido(pedido);

        if (response.HasError)
        {
            if (response.ErrorMessage is "Unauthorized" && !_loginPageDisplayed)
            {
                //Redirect to login page
                await DisplayLoginPage();
                return;


            }
            await DisplayAlert("Information", $"Something went wrong: {response.ErrorMessage}", "Cancel");
            return;
        }

        ItensCarrinhoCompra.Clear();
        LblEndereco.Text = "Add your address";
        LblPrecoTotal.Text = "0.00";


        await Navigation.PushAsync(new PedidoConfirmadoPage());

    }


}