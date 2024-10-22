using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches.Pages;

public partial class PedidoDetalhesPage : ContentPage
{
	private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;
    

    public PedidoDetalhesPage(ApiService service, IValidator validator, int pedidoId, decimal precoTotal)
    {
        InitializeComponent();
        _apiService = service;
        _validator = validator;


        LblPrecoTotal.Text = "$" + precoTotal;

        GetPedidoDetalhe(pedidoId);

    }

  

    private async void GetPedidoDetalhe(int pedidoId)
    {
        try
        {
            var (pedidoDetalhes, errorMessage) = await _apiService.GetPedidoDetalhes(pedidoId);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return;
            }
            if (pedidoDetalhes is null)
            {
                await DisplayAlert("Erro", errorMessage ?? "it wasn't possible to obtain the order details", "Ok");
                return;
            }
            else
            {
                CvPedidoDetalhes.ItemsSource = pedidoDetalhes;
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Erro", ex.Message, "Ok");
        }
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;

        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    

 
}