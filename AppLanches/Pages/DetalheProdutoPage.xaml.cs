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

    //Instance of the service to manage favorites
    private FavoritosService _favoritosService = new FavoritosService();

    private string? _imagemUrl;

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
        AtualizarFavoritoButton();
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
            _imagemUrl = produtoDetalhe.ImagePath;
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

    private async void ImagemBtnFavorito_Clicked(object sender, EventArgs e)
    {
        try
        {
            var existeFavorito = await _favoritosService.ReadAsync(_produtoId);
            if (existeFavorito != null)
            {
                await _favoritosService.DeleteAsync(existeFavorito);
            }
            else
            {
                var produtoFavorito = new ProdutoFavorito()
                {
                    ProdutoId = _produtoId,
                    IsFavorito = true,
                    Detalhe = LblDescricaoProduto.Text,
                    Nome = LblProdutoNome.Text,
                    Preco = decimal.Parse(LblProdutoPreco.Text),
                    ImagemUrl = _imagemUrl
                };
                await _favoritosService.CreateAsync(produtoFavorito);
            }

            AtualizarFavoritoButton();
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"Something went wrong: {ex.Message}", "OK");
        }
    }

    private async void AtualizarFavoritoButton()
    {
        var existeFavorito = await _favoritosService.ReadAsync(_produtoId);

        if (existeFavorito is not null)
        {
            ImagemBtnFavorito.Source = "heart_fill.png";
        }
        else
        {
            ImagemBtnFavorito.Source = "heart.png";
        }
    }

    private void BtnRemove_Clicked(object sender, EventArgs e)
    {
        if (int.TryParse(LblQuantidade.Text, out int quantidade) &&
            decimal.TryParse(LblProdutoPreco.Text, out decimal precoUnitario))
        {
            quantidade = Math.Max(1, quantidade - 1);
            LblQuantidade.Text = quantidade.ToString();

            var precoTotal = quantidade * precoUnitario;
            LblPrecoTotal.Text = precoTotal.ToString();
        }
        else
        {
            DisplayAlert("Error", "It was not possible to calculate the total price.", "OK");
        }
    }

    private void BtnAdiciona_Clicked(object sender, EventArgs e)
    {
        if (int.TryParse(LblQuantidade.Text, out int quantidade) &&
            decimal.TryParse(LblProdutoPreco.Text, out decimal precoUnitario))
        {
            quantidade++;
            LblQuantidade.Text = quantidade.ToString();

            var precoTotal = quantidade * precoUnitario;
            LblPrecoTotal.Text = precoTotal.ToString();
        }
        else
        {
            DisplayAlert("Error", "It was not possible to calculate the total price.", "OK");
        }
    }

    private async void BtnIncluirNoCarrinho_Clicked(object sender, EventArgs e)
    {
        try
        {
            var carrinhoCompra = new CarrinhoCompra()
            {
                ProdutoId = _produtoId,
                Quantidade = int.Parse(LblQuantidade.Text),
                PrecoUnitario = decimal.Parse(LblProdutoPreco.Text),
                ValorTotal = decimal.Parse(LblPrecoTotal.Text),
                ClienteId = Preferences.Get("usuarioid", 0)
            };

            var response = await _apiService.AdicionarItemNoCarrinho(carrinhoCompra);

            if (response.Data)
            {
                await DisplayAlert("Success", "Product added to cart.", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", response.ErrorMessage ?? "It was not possible to add the product to the cart.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Something went wrong: {ex.Message}", "OK");
        }
    }
}