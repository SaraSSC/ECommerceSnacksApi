using AppLanches.Pages;
using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches
{
    public partial class AppShell : Shell
    {
        private readonly ApiService _apiService;
        private readonly IValidator _validator;
        public AppShell(ApiService apiService, IValidator validator)
        {
            InitializeComponent();
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _validator = validator;

            ConfigureShell();

        }

        private void ConfigureShell()
        {
            var homePage = new HomePage(_apiService, _validator);
            var carrinhoPage = new CarrinhoPage(_apiService, _validator);
            var favoritosPage = new FavoritosPage();
            var perfilPage = new PerfilPage();

            Items.Add(new TabBar
            {
                Items =
                {
                    new ShellContent
                    {
                        Content = homePage,
                        Title = "Home",
                        Icon = "home.png"
                    },
                    new ShellContent
                    {
                        Content = carrinhoPage,
                        Title = "Carrinho",
                        Icon = "cart.png"
                    },
                    new ShellContent
                    {
                        Content = favoritosPage,
                        Title = "Favoritos",
                        Icon = "heart.png"
                    },
                    new ShellContent
                    {
                        Content = perfilPage,
                        Title = "Perfil",
                        Icon = "settings.png"
                    }
                }
            });
        }
    }
}
