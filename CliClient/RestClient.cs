using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using RestApi.Controllers;
using ServiceInterfaces.Dto;

namespace CliClient
{
    public class RestClient
    {
        public SsUser User { get; }
        public string BaseUri { get; }
        public RestClient(string baseUri, string userName, string password, bool ignoreServerCertificate = false)
        {
            BaseUri = baseUri;
            User = new SsUser(userName, password);

            if (ignoreServerCertificate)
            {
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (request, cert, chain, errors) => true;
                httpClient = new HttpClient(handler);
            }
            else
            {
                httpClient = new HttpClient();
            }
        }

        public async Task<IEnumerable<ResourceInfo>> GetResources()
        {
            var requestUri = $"{BaseUri}/stipistopi/resources";
            var stream = await httpClient.GetStreamAsync(requestUri);
            JsonSerializerOptions opts = new JsonSerializerOptions();
            opts.PropertyNameCaseInsensitive = true;
            var resources = await JsonSerializer.DeserializeAsync<IEnumerable<ResourceInfo>>(stream, opts);
            return resources;
        }

        private readonly HttpClient httpClient;
    }
}
