namespace frontend.Services;

using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl = "http://localhost:5030/api";

    public ApiService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_baseUrl)
        };
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }
        catch
        {
            return default;
        }
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(endpoint, data);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }
        catch
        {
            return default;
        }
    }

    public async Task<bool> PutAsync<T>(string endpoint, T data)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync(endpoint, data);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync(string endpoint)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(endpoint);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
