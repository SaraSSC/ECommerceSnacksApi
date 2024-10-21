using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches.Pages;

public partial class CarrinhoVazioPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    public CarrinhoVazioPage(ApiService apiService, IValidator validator)
	{
		InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

   

    private async void BtnRetornar_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}