using System.Collections.Generic;
using System.IO;
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
namespace RestClient
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

        public async Task<RestClientResult<SsUser>> AddUser(string userName, string password, UserRole role)
        {
            var request = new RestClientCommand<UserAndUserParameter, SsUser>(
                "/stipistopi/register",
                new UserAndUserParameter
                {
                    Creator = User,
                    User = new SsUser(userName, password, role)
                }
            );
            return await GenericRequest(request);
        }

        public async Task<RestClientResult<bool>> DelUser(string userName)
        {
            var request = new RestClientCommand<UserAndUserParameter, bool>(
                "/stipistopi/user/delete",
                new UserAndUserParameter
                {
                    Creator = User,
                    User = new SsUser { UserName = userName }
                }
            );
            return await GenericRequest(request);
        }

        public async Task<RestClientResult<SsResource>> AddResource(SsResource resource)
        {
            var request = new RestClientCommand<ResourceAndUserParameter, SsResource>(
                "/stipistopi/resource",
                new ResourceAndUserParameter
                {
                    Creator = User,
                    Resource = resource
                }
            );
            return await GenericRequest(request);
        }

        public Task<RestClientResult<bool>> ChangePassword(SsUser target)
        {
            var request = new RestClientCommand<UserAndUserParameter, bool>(
                "/stipistopi/user/password",
                new UserAndUserParameter
                {
                    User = target,
                    Creator = User,
                }
            );
            return GenericRequest(request);
        }

        public async Task<RestClientResult<bool>> DelResource(string shortName)
        {
            var request = new RestClientCommand<ResourceAndUserParameter, bool>(
                "/stipistopi/resource/delete",
                new ResourceAndUserParameter
                {
                    Creator = User,
                    Resource = new SsResource { ShortName = shortName }
                }
            );
            return await GenericRequest(request);
        }

        public async Task<RestClientResult<SsResource>> UpdateResourceDescription(
            string resourceName, string oldDescription, string newDescription)
        {
            var request = new RestClientCommand<ResourceDescriptionParameter, SsResource>(
                "/stipistopi/resource/description",
                new ResourceDescriptionParameter
                {
                    ResourceName = resourceName,
                    OldDescription = oldDescription,
                    NewDescription = newDescription,
                    User = User,
                }
            );
            return await GenericRequest(request);
        }

        public async Task<IEnumerable<SsResource>> GetResources()
        {
            var requestUri = GetUri("/stipistopi/resources");
            var stream = await HttpClient.GetStreamAsync(requestUri);
            return await JsonSerializer.DeserializeAsync<IEnumerable<SsResource>>(stream, JsonOptions);
        }

        public async Task<RestClientResult<IEnumerable<SsUser>>> GetUsers()
        {
            return await GenericRequest(
                new RestClientCommand<SsUser, IEnumerable<SsUser>>(
                    "/stipistopi/users",
                    User));
        }

        public async Task<RestClientResult<bool>> LockOperation(string type, string resourceName)
        {
            return await GenericRequest(
                new RestClientCommand<LockParameter, bool>(
                    $"/stipistopi/{type}",
                    new LockParameter
                    {
                        ResourceName = resourceName,
                        User = User,
                    }
                )
            );
        }

        public async Task<RestClientResult<string>> DbExport()
        {
            return await GenericRequest(
                new RestClientCommand<SsUser, string>(
                    "/stipistopi/db/export",
                    User)
            );
        }

        public async Task<RestClientResult<bool>> DbImport(string content)
        {
            return await GenericRequest(
                new RestClientCommand<DbImportParameter, bool>(
                    "/stipistopi/db/import",
                    new DbImportParameter
                    {
                        User = User,
                        Content = content
                    })
            );
        }

        public async Task<RestClientResult<TResponse>> GenericRequest<TRequest, TResponse>(RestClientCommand<TRequest, TResponse> restClientCommand)
        {
            var requestUri = GetUri(restClientCommand.Uri);
            var content = new StringContent(JsonSerializer.Serialize(restClientCommand.RequestParam), Encoding.UTF8, "application/json");
            var response = await HttpClient.PostAsync(requestUri, content);
            var stream = await response.Content.ReadAsStreamAsync();
            if (response.StatusCode != HttpStatusCode.OK)
            {
                try
                {
                    var error = await JsonSerializer.DeserializeAsync<RestError>(stream, JsonOptions);
                    return new RestClientResult<TResponse>(error);
                }
                catch (JsonException)
                {
                    return new RestClientResult<TResponse>(new RestError
                    {
                        Message = $"There has been an unexpected error. Status code: {response.StatusCode}"
                    });
                }
            }
            if (typeof(TResponse) == typeof(string))
            {
                object str = new StreamReader(stream).ReadToEnd();
                return new RestClientResult<TResponse>((TResponse)str);
            }
            var result = await JsonSerializer.DeserializeAsync<TResponse>(stream, JsonOptions);
            return new RestClientResult<TResponse>(result);
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