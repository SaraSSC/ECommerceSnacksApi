
using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches.Pages;

public partial class InscricaoPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
	public InscricaoPage(ApiService apiService, IValidator validator)
	{
		InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }


    private async void BtnSignup_Clicked(object sender, EventArgs e)
    {
        if (await _validator.Validate(EntNome.Text, EntEmail.Text, EntPhone.Text, EntPassword.Text))
        {
            var response = await _apiService.UserRegister(EntNome.Text, EntEmail.Text, EntPhone.Text, EntPassword.Text);

            if (!response.HasError)
            {
                await DisplayAlert("Alert", "Welcome to the Team !! :)", "OK");
                await Navigation.PushAsync(new LoginPage(_apiService, _validator));
            }
            else
            {
                await DisplayAlert("Error", $"Something is wrong!! :( \nError: {response.ErrorMessage}", "Cancel");
            }
        }
        else
        {
            string messageError = "";
            messageError += _validator.NomeError != null ? $"\n- {_validator.NomeError}" : "";
            messageError += _validator.EmailError != null ? $"\n- {_validator.EmailError}" : "";
            messageError += _validator.TelefoneError != null ? $"\n- {_validator.TelefoneError}" : "";
            messageError += _validator.SenhaError != null ? $"\n- {_validator.SenhaError}" : "";

            await DisplayAlert("Error", messageError, "OK");
        }
    }

    private async void TapLogin_Tapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new LoginPage(_apiService, _validator));
        }
    
}