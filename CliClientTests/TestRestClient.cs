using CliClient;
using LiteDbSsRepositoryService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using RestApi;
using ServiceInterfaces;
using ServiceInterfaces.Dto;

namespace CliClientTests
{
    public class TestRestClient
    {
        public RestClient RestClient { get; }
        public SsUser Admin { get; }
        public TestRestClient()
        {
            Admin = new SsUser("admin", "admin", UserRole.Admin);
            RestClient = InitRestClient(Admin);
        }
        private RestClient InitRestClient(SsUser admin)
        {
            var repo = new LiteDbSsRepository();
            repo.Transaction(() => repo.SaveUser(new SsUserSecret(admin)));
            var webHostBuilder = new WebHostBuilder().UseStartup<Startup>();
            webHostBuilder.ConfigureServices(services =>
            {
                services.Add(new Microsoft.Extensions.DependencyInjection.ServiceDescriptor(
                    typeof(ISsRepository), sp => repo, ServiceLifetime.Singleton
                ));
            });
            var testServer = new TestServer(webHostBuilder);
            var restHttpClient = new TestRestHttpClient(testServer.CreateClient());
            var restClient = new RestClient(restHttpClient, Admin.UserName, Admin.Password);
            return restClient;
        }
    }
}
