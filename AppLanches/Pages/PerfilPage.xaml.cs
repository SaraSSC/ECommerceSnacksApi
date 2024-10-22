using AppLanches.Models;
using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches.Pages;

public partial class PerfilPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;
    public PerfilPage(ApiService apiService, IValidator validator)
	{
		InitializeComponent();
        LblNomeUsuario.Text = Preferences.Get("usuarionome", string.Empty);
        _apiService = apiService;
        _validator = validator;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        ImgBtnPerfil.Source = await GetImagemPerfil();
    }

    private async Task<ImageSource> GetImagemPerfil()
    {
        string imagemPadrao = AppConfig.PerfilImagemPadrao;

        var (response, errorMessage) = await _apiService.GetImagemPerfilUsuario();

        if (errorMessage is not null)
        {
            switch (errorMessage)
            {
                case "Unauthorized":
                    if (!_loginPageDisplayed)
                    {
                        await DisplayLoginPage();
                        return null;
                    }
                    break;
                default:
                    await DisplayAlert("Error", errorMessage ?? "It wasn't possibile to obtain the image", "OK");
                    return imagemPadrao;
            }
        }
        if (response?.UrlImagem is not null)
        {
            return response.ImagePath;
        }
        return imagemPadrao;
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    private void TapMinhaConta_Tapped(object sender, TappedEventArgs e)
    {

    }

    private void TapPerguntas_Tapped(object sender, TappedEventArgs e)
    {

    }

    private void TapPedidos_Tapped(object sender, TappedEventArgs e)
    {
        Navigation.PushAsync(new PedidoPage(_apiService, _validator));

    }

    private void BtnLogout_Clicked(object sender, EventArgs e)
    {

    }

    private async void ImgBtnPerfil_Clicked(object sender, EventArgs e)
    {
        try
        {
            var imagemArray = await SelecionarImagemAsync();
            if (imagemArray is null)
            {
                await DisplayAlert("Error", "It wasn't possible to upload the picture", "OK");
                return;
            }

            ImgBtnPerfil.Source = ImageSource.FromStream(() => new MemoryStream(imagemArray));

            var response = await _apiService.UploadImagemUsuario(imagemArray);
            if (response.Data)
            {
                await DisplayAlert("", "Picture successefully sent", "OK");
            }
            else
            {
                await DisplayAlert("Error", response.ErrorMessage, "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async Task<byte[]?> SelecionarImagemAsync()
    {
        try
        {
            var arquivo = await MediaPicker.PickPhotoAsync();

            if (arquivo is null)
            {
                return null;
            }

            using (var stream = await arquivo.OpenReadAsync())

            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
        catch (FeatureNotSupportedException)
        {
            await DisplayAlert("Error", "Feature not supported", "OK");

        }
        catch (PermissionException)
        {
            await DisplayAlert("Error", "Permission denied", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
        return null;
    }
}