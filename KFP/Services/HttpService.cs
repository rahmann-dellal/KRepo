using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KFP.Services
{
    public class HttpService
    {
        public readonly HttpClient _httpClient;

        public HttpService()
        {
            _httpClient = new HttpClient()
            {
                Timeout = TimeSpan.FromSeconds(3)
            };
        }

        private static readonly string[] TrustedUrls =
    {
        "https://www.google.com/generate_204",
        "https://www.cloudflare.com",
        "https://www.microsoft.com"
    };

        public static async Task<bool> IsInternetAvailableAsync()
        {
            using var httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(3)
            };

            foreach (var url in TrustedUrls)
            {
                try
                {
                    var response = await httpClient.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                        return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        public static async Task<bool> IsKioberServerAvailableAsync()
        {
            using var httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(3)
            };
            try
            {
                var response = await httpClient.GetAsync("https://app.kiober.com/CheckServerAvailable");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Sends a POST request.
        /// </summary>
        /// <typeparam name="T">The type to deserialize.</typeparam>
        /// <param name="url">The endpoint URL.</param>
        /// <param name="data">The request body.</param>
        /// <returns>The deserialized response.</returns>
        /// <exception cref="NetworkException">Thrown if the server cannot be reached.</exception>
        /// <exception cref="RequestTimeoutException">Thrown if the request times out.</exception>
        /// <exception cref="ClientException">Thrown if the server returns 4xx.</exception>
        /// <exception cref="ServerException">Thrown if the server returns 5xx.</exception>
        public async Task<T?> PostAsync<T>(string url, object data)
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                using var response = await _httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();

                    if ((int)response.StatusCode >= 500)
                        throw new ServerException(
                            $"Server error: {(int)response.StatusCode} {response.ReasonPhrase}",
                            (int)response.StatusCode,
                            responseBody
                        );

                    if ((int)response.StatusCode >= 400)
                        throw new ClientException(
                            $"Client error: {(int)response.StatusCode} {response.ReasonPhrase}",
                            (int)response.StatusCode,
                            responseBody
                        );
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                return JsonSerializer.Deserialize<T>(responseContent, options);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == null)
            {
                throw new NetworkException("Network error: Unable to reach server.", ex);
            }
            catch (TaskCanceledException ex)
            {
                throw new RequestTimeoutException("The request timed out.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error occurred while sending the request.", ex);
            }
        }
    }
    public class NetworkException : Exception
    {
        public NetworkException(string message, Exception? inner = null)
            : base(message, inner) { }
    }

    public class ServerException : Exception
    {
        public int StatusCode { get; }
        public string? ResponseBody { get; }

        public ServerException(string message, int statusCode, string? responseBody = null, Exception? inner = null)
            : base(message, inner)
        {
            StatusCode = statusCode;
            ResponseBody = responseBody;
        }
    }

    public class ClientException : Exception
    {
        public int StatusCode { get; }
        public string? ResponseBody { get; }

        public ClientException(string message, int statusCode, string? responseBody = null, Exception? inner = null)
            : base(message, inner)
        {
            StatusCode = statusCode;
            ResponseBody = responseBody;
        }
    }

    public class RequestTimeoutException : Exception
    {
        public RequestTimeoutException(string message, Exception? inner = null)
            : base(message, inner) { }
    }
}
