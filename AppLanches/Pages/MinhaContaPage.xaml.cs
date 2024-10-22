using AppLanches.Models;
using AppLanches.Services;


namespace AppLanches.Pages;

public partial class MinhaContaPage : ContentPage
{
	private readonly ApiService _apiService;

    private const string NomeUsuarioKey = "usuarionome";

    private const string EmailUsuarioKey = "usuarioemail";

    private const string TelefoneUsuarioKey = "usuariotelefone";

    public MinhaContaPage(ApiService apiService)
	{
		InitializeComponent();
        _apiService = apiService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        CarregarInformacoesUsuario();
        ImgBtnPerfil.Source = await GetImagemPerfilAsync();

    }

    private async Task<ImageSource> GetImagemPerfilAsync()
    {
        string imagemPadrao = AppConfig.PerfilImagemPadrao;

        var (response, errorMessage) = await _apiService.GetImagemPerfilUsuario();

        if (errorMessage is not null)
        {
            switch (errorMessage)
            {
                case "Unauthorized":
                    await DisplayAlert("Error", "Unauthorized", "OK");
                    return imagemPadrao;
                default:
                    await DisplayAlert("Error", errorMessage ?? "Wasn't possible to obtain the picture", "OK");
                    return imagemPadrao;
            }
        }
        if (response?.UrlImagem is not null)
        {
            return response.ImagePath;
        }
        return imagemPadrao;
    }

    private void CarregarInformacoesUsuario()
    {
        LblNomeUsuario.Text = Preferences.Get(NomeUsuarioKey, string.Empty);
        EntNome.Text = LblNomeUsuario.Text;
        EntEmail.Text = Preferences.Get(EmailUsuarioKey, string.Empty);
        EntTelefone.Text = Preferences.Get(TelefoneUsuarioKey, string.Empty);
    }

    private async void BtnSalvar_Clicked(object sender, EventArgs e)
    {
        Preferences.Set(NomeUsuarioKey, EntNome.Text);
        Preferences.Set(EmailUsuarioKey, EntEmail.Text);
        Preferences.Set(TelefoneUsuarioKey, EntTelefone.Text);
        await DisplayAlert("Information safe", "Your information has been saved successefully", "OK");
    }
}