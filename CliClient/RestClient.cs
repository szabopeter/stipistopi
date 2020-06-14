using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using RestApi.Controllers;
using ServiceInterfaces.Dto;

namespace CliClient
{

    public class RestClient
    {
        public IRestHttpClient RestHttpClient { get; }
        public string BaseUri { get; }
        public SsUser User { get; }

        private HttpClient httpClient => RestHttpClient.HttpClient;

        public RestClient(IRestHttpClient restHttpClient, string userName, string password)
        {
            RestHttpClient = restHttpClient;
            User = new SsUser(userName, password);
        }

        private string GetUri(string withoutBase) 
        {
            return $"{RestHttpClient.BaseUri}{withoutBase}";
        }

        public async Task AddUser(string userName, string password, UserRole role)
        {
            var requestUri = GetUri("/stipistopi/register");
            var requestParam = new NewUserParameter
            {
                Creator = User,
                User = new SsUser(userName, password, role)
            };
            RestHttpClient.WriteLine("Dispatching request...");
            var content = new StringContent(JsonSerializer.Serialize(requestParam), Encoding.UTF8, "application/json");
            var result = await httpClient.PostAsync(requestUri, content);
            RestHttpClient.WriteLine($"Server responded with {result.StatusCode}");
        }

        public async Task<IEnumerable<ResourceInfo>> GetResources()
        {
            var requestUri = GetUri("/stipistopi/resources");
            var stream = await httpClient.GetStreamAsync(requestUri);
            JsonSerializerOptions opts = new JsonSerializerOptions();
            opts.PropertyNameCaseInsensitive = true;
            var resources = await JsonSerializer.DeserializeAsync<IEnumerable<ResourceInfo>>(stream, opts);
            return resources;
        }

        public async Task<IEnumerable<SsUser>> GetUsers()
        {
            var requestUri = GetUri("/stipistopi/users");
            var requestParam = User;
            var content = new StringContent(JsonSerializer.Serialize(requestParam), Encoding.UTF8, "application/json");
            var result = await httpClient.PostAsync(requestUri, content);
            Console.WriteLine(result.StatusCode);
            if (result.StatusCode != HttpStatusCode.OK)
                return Array.Empty<SsUser>();

            var stream = await result.Content.ReadAsStreamAsync();
            // TODO: use the same options for all requests
            JsonSerializerOptions opts = new JsonSerializerOptions();
            opts.PropertyNameCaseInsensitive = true;
            opts.Converters.Add(new JsonStringEnumConverter());
            var resources = await JsonSerializer.DeserializeAsync<IEnumerable<SsUser>>(stream, opts);
            return resources;
        }
    }
}
