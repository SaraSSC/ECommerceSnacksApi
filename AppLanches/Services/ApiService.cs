using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public ApiService(HttpClient httpClient,
            ILogger<ApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<ApiResponse<bool>> UserRegister(string name, string email, string phone, string password)
        {
            try
            {
                var register = new Register()
                {
                    Nome = name,
                    Email = email,
                    Telefone = phone,
                    Senha = password
                };

                var json = JsonSerializer.Serialize(register, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("api/Users/Register", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Erro ao enviar requisição HTTP: {response.StatusCode}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Erro ao enviar requisição HTTP: {response.StatusCode}"
                    };
                }

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao registar o user: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }

        }

        public async Task<ApiResponse<bool>> Login(string email, string password)
        {
            try
            {
                var login = new Login()
                {
                    Email = email,
                    Senha = password
                };

                var json = JsonSerializer.Serialize(login, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("api/Users/Login", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Erro ao enviar requisição HTTP: {response.StatusCode}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Erro ao enviar requisição HTTP: {response.StatusCode}"
                    };
                }

                var jsonResult = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Token>(jsonResult, _serializerOptions);

                Preferences.Set("accesstoken", result!.AccessToken);
                Preferences.Set("usuarioid", (int)result.UsuarioId!);
                Preferences.Set("usuarionome", result.UsuarioNome);

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to login {ex.Message}");
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
                _logger.LogError($"Erro ao enviar requisição POST para {uri}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

        }

        //public async Task<(List<Category>? Categorias, string? ErrorMessage)> GetCategories()
        //{
        //    return await GetAsync<List<Category>>("api/categories");
        //}

        //public async Task<(List<Product>? Produtos, string? ErrorMessage)> GetProducts(string typeProduct, string categoryId)
        //{
        //    string endpoint = $"api/Products?typeProduct={typeProduct}&categoryId={categoryId}";
        //    return await GetAsync<List<Product>>(endpoint);
        //}

        //private async Task<(T? Data, string? ErrorMessage)> GetAsync<T>(string endpoint)
        //{
        //    try
        //    {
        //        AddAuthorizationHeader();

        //        var response = await _httpClient.GetAsync(AppConfig.BaseUrl + endpoint);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            var responseString = await response.Content.ReadAsStringAsync();
        //            var data = JsonSerializer.Deserialize<T>(responseString, _serializerOptions);
        //            return (data ?? Activator.CreateInstance<T>(), null);
        //        }
        //        else
        //        {
        //            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        //            {
        //                string errorMessage = "Unauthorized";
        //                _logger.LogWarning(errorMessage);
        //                return (default, errorMessage);
        //            }

        //            string generalErrorMessage = $"Erro na requisição: {response.ReasonPhrase}";
        //            _logger.LogError(generalErrorMessage);
        //            return (default, generalErrorMessage);
        //        }
        //    }
        //    catch (HttpRequestException ex)
        //    {
        //        string errorMessage = $"Erro de requisição HTTP: {ex.Message}";
        //        _logger.LogError(ex, errorMessage);
        //        return (default, errorMessage);
        //    }
        //    catch (JsonException ex)
        //    {
        //        string errorMessage = $"Erro de desserialização JSON: {ex.Message}";
        //        _logger.LogError(ex, errorMessage);
        //        return (default, errorMessage);
        //    }
        //    catch (Exception ex)
        //    {
        //        string errorMessage = $"Erro inesperado: {ex.Message}";
        //        _logger.LogError(ex, errorMessage);
        //        return (default, errorMessage);
        //    }
        //}

        private void AddAuthorizationHeader()
        {
            var token = Preferences.Get("accesstoken", string.Empty);
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

    }
}

