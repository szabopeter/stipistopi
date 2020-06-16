using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using RestApi;
using RestApi.Controllers;
using ServiceInterfaces.Dto;

#pragma warning disable RCS1090
namespace CliClient
{
    public class RestClient
    {
        public IRestHttpClient RestHttpClient { get; }
        public string BaseUri { get; }
        public SsUser User { get; }

        private HttpClient HttpClient => RestHttpClient.HttpClient;

        private readonly JsonSerializerOptions JsonOptions;

        public RestClient(IRestHttpClient restHttpClient, string userName, string password)
        {
            RestHttpClient = restHttpClient;
            User = new SsUser(userName, password);
            JsonOptions = CreateJsonOptions();
        }

        public async Task<RestError> AddUser(string userName, string password, UserRole role)
        {
            var requestUri = GetUri("/stipistopi/register");
            var requestParam = new NewUserParameter
            {
                Creator = User,
                User = new SsUser(userName, password, role)
            };
            RestHttpClient.WriteLine("Dispatching request...");
            var content = new StringContent(JsonSerializer.Serialize(requestParam), Encoding.UTF8, "application/json");
            var result = await HttpClient.PostAsync(requestUri, content);
            RestHttpClient.WriteLine($"Server responded with {result.StatusCode}");
            if (result.StatusCode == HttpStatusCode.OK)
                return null;
            var stream = await result.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<RestError>(stream, JsonOptions);
        }

        public async Task<IEnumerable<ResourceInfo>> GetResources()
        {
            var requestUri = GetUri("/stipistopi/resources");
            var stream = await HttpClient.GetStreamAsync(requestUri);
            return await JsonSerializer.DeserializeAsync<IEnumerable<ResourceInfo>>(stream, JsonOptions);
        }

        public async Task<IEnumerable<SsUser>> GetUsers()
        {
            var requestUri = GetUri("/stipistopi/users");
            var requestParam = User;
            var content = new StringContent(JsonSerializer.Serialize(requestParam), Encoding.UTF8, "application/json");
            var result = await HttpClient.PostAsync(requestUri, content);
            Console.WriteLine(result.StatusCode);
            if (result.StatusCode != HttpStatusCode.OK)
                return Array.Empty<SsUser>();

            var stream = await result.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<IEnumerable<SsUser>>(stream, JsonOptions);
        }

        private static JsonSerializerOptions CreateJsonOptions()
        {
            var opts = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            opts.Converters.Add(new JsonStringEnumConverter());
            return opts;
        }

        private string GetUri(string withoutBase)
        {
            return RestHttpClient.BaseUri + withoutBase;
        }
    }
}
#pragma warning restore