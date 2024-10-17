using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AppLanches.Models;


namespace AppLanches.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiService> _logger;
        private readonly string _baseUrl = "https://dzzqk7t1-5113.uks1.devtunnels.ms/";

        JsonSerializerOptions _serializerOptions;
        public ApiService(HttpClient httpClient, ILogger<ApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<ApiResponse<bool>> UserRegister(string nome, string email, string telefone, string senha)
        {
            try
            {
                var register = new Register()
                {
                    Nome = nome,
                    Email = email,
                    Telefone = telefone,
                    Senha = senha
                };

                var json = JsonSerializer.Serialize(register, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("api/Usuarios/Register", content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Error sending the HTTP request: {response.StatusCode} - {error}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = ($"Error sending the HTTP request: {response.StatusCode} - {error}")
                    };
                }

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while register the user: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> Login(string email, string senha)
        {
            try
            {
                var login = new Login()
                {
                    Email = email,
                    Senha = senha
                };

                var json = JsonSerializer.Serialize(login, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("api/Usuarios/Login", content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Error sending the HTTP request: {response.StatusCode} - {error}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = ($"Error sending the HTTP request: {response.StatusCode} - {error}")
                    };
                }

                var jsonResult = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(jsonResult))
                {
                    _logger.LogError("The response content is empty.");
                    return new ApiResponse<bool> { ErrorMessage = "The response content is empty." };
                }

                var result = JsonSerializer.Deserialize<Token>(jsonResult, _serializerOptions);

                if (result == null)
                {
                    _logger.LogError("Failed to deserialize the token.");
                    return new ApiResponse<bool> { ErrorMessage = "Failed to deserialize the token." };
                }

                Preferences.Set("accesstoken", result.AccessToken);
                Preferences.Set("usuarioid", (int)result.UsuarioId!);
                Preferences.Set("usuarionome", result.UsuarioNome);

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro while register the user: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        public async Task<HttpResponseMessage> PostRequest(string uri, HttpContent content)
        {
            var Url = _baseUrl + uri;
            try
            {
                var result = await _httpClient.PostAsync(Url, content);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending request: POST to {uri}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        private void AddAuthorizationHeader()
        {
            var token = Preferences.Get("accesstoken", string.Empty);
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<(List<Categoria>? Categorias, string? ErrorMessage)> GetCategorias()
        {
            return await GetAsync<List<Categoria>>("api/Categorias");
        }

        public async Task<(List<Produto>? Produtos, string? ErrorMessage)> GetProdutos(string tipoProduto, string categoriaId)
        {
            string endpoint = $"api/Produtos?tipoProduto={tipoProduto}&categoriaId={categoriaId}"; //Under Product? get tipoProduto & categoriaId
            return await GetAsync<List<Produto>>(endpoint);
        }

        private async Task<(T? Data, string? ErrorMessage)> GetAsync<T>(string endpoint)
        {
            try
            {
                AddAuthorizationHeader();
                var response = await _httpClient.GetAsync(AppConfig.BaseUrl + endpoint);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<T>(jsonResult, _serializerOptions);
                    return (result ?? Activator.CreateInstance<T>(), null);
                }
                else
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        string errorMessage = "Unauthorized";
                        _logger.LogWarning(errorMessage);
                        return (default, "Unauthorized");
                    }
                    string generalErrorMessage = $"Error sending the HTTP request: {response.ReasonPhrase}";
                    _logger.LogError(generalErrorMessage);
                    return (default, generalErrorMessage);
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"Error sending the HTTP request: {ex.Message}");
                return (default, ex.Message);
            }
            catch (JsonException ex)
            {
                _logger.LogError($"Error deserializing the response: {ex.Message}");
                return (default, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return (default, ex.Message);
            }
        }

        internal async Task<(Produto? produtoDetalhe, string errorMessage)> GetDetalheProduto(int produtoId)
        {
            string endpoint = $"api/Produtos/{produtoId}";
            return await GetAsync<Produto>(endpoint);
        }

        public async Task<ApiResponse<bool>> AdicionarItemNoCarrinho(CarrinhoCompra carrinhoCompra)
        {
            try
            {
                var json = JsonSerializer.Serialize(carrinhoCompra, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("api/ItensCarrinhoCompra", content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Error sending the HTTP request: {response.StatusCode} - {error}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = ($"Error sending the HTTP request: {response.StatusCode} - {error}")
                    };
                }

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while adding item to cart : {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        public async Task<(List<CarrinhoCompraItem>? CarrinhoCompraItems, string? ErrorMessage)> GetItensCarrinhoCompra(int usuarioId)
        {
            var endpoint = $"api/ItensCarrinhoCompra/{usuarioId}";
            return await GetAsync<List<CarrinhoCompraItem>>(endpoint);
        }
    }
}
