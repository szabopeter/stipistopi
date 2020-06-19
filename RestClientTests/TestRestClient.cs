using RestClient;
using LiteDbSsRepositoryService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using RestApi;
using ServiceInterfaces;
using ServiceInterfaces.Dto;

namespace RestClientTests
{
    public class TestRestClient
    {
        public RestClient.RestClient RestClient { get; }
        public SsUser Admin { get; }
        public SsUser User { get; }
        private TestRestHttpClient RestHttpClient { get; }

        public TestRestClient(SsUser user = null)
        {
            Admin = new SsUser("admin", "admin", UserRole.Admin);
            User = user ?? Admin;
            RestHttpClient = InitRestHttpClient(Admin, User);
            RestClient = new RestClient.RestClient(RestHttpClient, User.UserName, User.Password);
        }

        public RestClient.RestClient GetAdditionalRestClient(string userName, string password)
        {
            return new RestClient.RestClient(RestHttpClient, userName, password);
        }

        private static TestRestHttpClient InitRestHttpClient(SsUser admin, SsUser user)
        {
            var repo = new LiteDbSsRepository();
            repo.Transaction(() => repo.SaveUser(new SsUserSecret(admin)));
            if (user != admin)
                repo.Transaction(() => repo.SaveUser(new SsUserSecret(user)));
            var webHostBuilder = new WebHostBuilder().UseStartup<Startup>();
            webHostBuilder.ConfigureServices(services =>
            {
                services.Add(new ServiceDescriptor(
                    typeof(ISsRepository), sp => repo, ServiceLifetime.Singleton
                ));
            });
            var testServer = new TestServer(webHostBuilder);
            return new TestRestHttpClient(testServer.CreateClient());
        }
    }
}
