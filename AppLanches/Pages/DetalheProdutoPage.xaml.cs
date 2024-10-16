using AppLanches.Models;
using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches.Pages;

public partial class DetalheProdutoPage : ContentPage
{
	private readonly ApiService _apiService;
    private readonly IValidator _validator;
	private int _produtoId;
	private bool _loginPageDisplayed = false;

    public DetalheProdutoPage(int produtoId, string produtoNome, ApiService apiService, IValidator validator)
	{
		InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        _produtoId = produtoId;
        Title = produtoNome ?? "Product Details";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetDetalheProduto(_produtoId);
    }

    private async Task<Produto?> GetDetalheProduto(int produtoId)
    {
        var (produtoDetalhe, errorMessage) = await _apiService.GetDetalheProduto(produtoId);

        if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
        {
            await DisplayLoginPage();
            return null;
        }

        if (produtoDetalhe is null)
        {
            await DisplayAlert("Error", errorMessage ?? "It was not possible to obtain the product details.", "OK");
            return null;
        }

        if (produtoDetalhe != null)
        {
            LblProdutoNome.Text = produtoDetalhe.Nome;
            LblDescricaoProduto.Text = produtoDetalhe.Detalhe;
            LblProdutoPreco.Text = produtoDetalhe.Preco.ToString();
            ImageProduct.Source = produtoDetalhe.ImagePath;
            LblPrecoTotal.Text = produtoDetalhe.Preco.ToString();
        }
        else
        {
            await DisplayAlert("Error", errorMessage ?? "It was not possible to obtain the product details.", "OK");
        }
        return produtoDetalhe;
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    private void ImagemBtnFavorito_Clicked(object sender, EventArgs e)
    {

    }

    private void BtnRemove_Clicked(object sender, EventArgs e)
    {

    }

    private void BtnAdiciona_Clicked(object sender, EventArgs e)
    {

    }

    private void BtnIncluirNoCarrinho_Clicked(object sender, EventArgs e)
    {

    }
}