using System;
using System.Linq;
using CliClient;
using LiteDbSsRepositoryService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using RestApi;
using ServiceInterfaces;
using ServiceInterfaces.Dto;
using Xunit;

namespace CliClientTests
{

    public class AddUserCommandTest
    {
        [Fact]
        public async void TestAddAndList()
        {
            var restClient = InitRestClient();
            // TODO: create resources test class
            var resources = await restClient.GetResources();
            Assert.Empty(resources);
            await restClient.AddUser("newUserName", "newUserPassword", UserRole.Admin);
            var userList = await restClient.GetUsers();
            var newUser = userList.Single(u =>
                string.Equals(u.UserName, "newUserName", StringComparison.InvariantCultureIgnoreCase));
            Assert.Equal(UserRole.Admin, newUser.Role);
        }

        private RestClient InitRestClient()
        {
            var repo = new LiteDbSsRepository();
            repo.Transaction(() => repo.SaveUser(new SsUserSecret(Admin)));
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

        public SsUser Admin { get; } = new SsUser("admin", "admin", UserRole.Admin);
    }
}
