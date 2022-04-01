using WebCon.ImportSubstitutionsApplication.Interfaces;
using WebCon.ImportSubstitutionsApplication.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;

namespace WebCon.ImportSubstitutionsApplication.Managers
{
    public class RestClient : IRestClient
    {
        private readonly IConfigurationSettings _configurationSettings;
        private readonly HttpClient _client;
        private const string AuthorizationTokenEndpoint = "api/login";

        public RestClient(IConfigurationSettings configurationSettings)
        {
            _configurationSettings = configurationSettings;
            _client = new HttpClient()
            {
                BaseAddress = new Uri(_configurationSettings.PortalUrl.EndsWith(@"/") ? _configurationSettings.PortalUrl : _configurationSettings.PortalUrl + @"/")
            };
        }
        
        public HttpResponseMessage Get(string endpoint)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken());
            var response = _client.GetAsync(endpoint).Result;
            if (response.IsSuccessStatusCode)
            {
                return response;
            }
            var responseContent = response.Content?.ReadAsStringAsync().Result;
            throw new HttpRequestException($"Cannot connect to ${new Uri(_client.BaseAddress, endpoint)} properly. Retrieved content {responseContent} with status code {response.StatusCode}.");

        }

        public void Send(string endpoint, string data)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(endpoint, UriKind.Relative),
                Content = new StringContent(data, Encoding.UTF8, "application/json")
            };
            var response = _client.SendAsync(request).Result;
            if (response.IsSuccessStatusCode)
            {
                return;
            }
            var responseContent = response.Content?.ReadAsStringAsync().Result;
            throw new HttpRequestException($"{response.StatusCode} (Content: {responseContent})");
        }

        private string GetToken()
        {
            var loginData = new
            {
                _configurationSettings.ClientId,
                _configurationSettings.ClientSecret,
            };

            var authData = new StringContent(JsonConvert.SerializeObject(loginData), Encoding.UTF8, "application/json");
            var response = _client.PostAsync(AuthorizationTokenEndpoint, authData).Result;
            var responseContent = response.Content?.ReadAsStringAsync().Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Cannot get access token. Retrieved content {responseContent} with status code {response.StatusCode}.");
            }

            return JsonConvert.DeserializeObject<BearerToken>(responseContent)?.Token;
        }
    }
}
