namespace frontend.Services;

using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl = "http://localhost:5130/api";

    public ApiService()
    {
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };
        
        // Configurar headers por defecto
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        try
        {
            var fullUrl = $"{_baseUrl}{endpoint}";
            Console.WriteLine($"🔥 GET {fullUrl}");
            var response = await _httpClient.GetAsync(fullUrl);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(content))
                    return default;
                    
                return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            
            return default;
        }
        catch (HttpRequestException)
        {
            // Error de conexión
            return default;
        }
        catch (TaskCanceledException)
        {
            // Timeout
            return default;
        }
        catch (JsonException)
        {
            // Error de deserialización
            return default;
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
            var fullUrl = $"{_baseUrl}{endpoint}";
            Console.WriteLine($"� POST  {fullUrl}");
            Console.WriteLine($"📦 Data: {JsonSerializer.Serialize(data)}");
            
            var response = await _httpClient.PostAsJsonAsync(fullUrl, data, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            
            Console.WriteLine($"✅ Status: {response.StatusCode}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"📥 Response: {content}");
                
                if (string.IsNullOrEmpty(content))
                    return default;
                    
                return JsonSerializer.Deserialize<TResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ Error Response: {errorContent}");
            }
            
            return default;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"❌ HttpRequestException: {ex.Message}");
            return default;
        }
        catch (TaskCanceledException ex)
        {
            Console.WriteLine($"❌ TaskCanceledException: {ex.Message}");
            return default;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"❌ JsonException: {ex.Message}");
            return default;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Exception: {ex.Message}");
            return default;
        }
    }

    public async Task<bool> PutAsync<T>(string endpoint, T data)
    {
        try
        {
            var fullUrl = $"{_baseUrl}{endpoint}";
            Console.WriteLine($"🔥 PUT {fullUrl}");
            var response = await _httpClient.PutAsJsonAsync(fullUrl, data, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException)
        {
            return false;
        }
        catch (TaskCanceledException)
        {
            return false;
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
            var fullUrl = $"{_baseUrl}{endpoint}";
            Console.WriteLine($"🔥 DELETE {fullUrl}");
            var response = await _httpClient.DeleteAsync(fullUrl);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException)
        {
            return false;
        }
        catch (TaskCanceledException)
        {
            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            var healthUrl = "http://localhost:5130/api/health";
            Console.WriteLine($"🏥 Testing connection to {healthUrl}");
            var response = await _httpClient.GetAsync(healthUrl);
            Console.WriteLine($"🏥 Health check status: {response.StatusCode}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Health check failed: {ex.Message}");
            return false;
        }
    }

    public async Task<byte[]?> DownloadPdfAsync(string endpoint)
    {
        try
        {
            var fullUrl = $"{_baseUrl}{endpoint}";
            Console.WriteLine($"📄 Downloading PDF from {fullUrl}");
            var response = await _httpClient.GetAsync(fullUrl);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            
            Console.WriteLine($"❌ PDF download failed: {response.StatusCode}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ PDF download exception: {ex.Message}");
            return null;
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}
