using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using RestApi;
using RestApi.Controllers;
using ServiceInterfaces.Dto;
using Xunit;

namespace CliClientTests
{
    public class AddUserCommandTest
    {
        [Fact]
        public async void TestAddAndList()
        {
            var webHostBuilder = new WebHostBuilder().UseStartup<Startup>();
            var testServer = new TestServer(webHostBuilder);
            var client = testServer.CreateClient();

            var requestUri = $"/stipistopi/register";
            var requestParam = new NewUserParameter
            {
                // TODO There should be a common source for default admin credentials used here and in Populate 
                Creator = new SsUser("test", "test"),
                User = new SsUser("newUserName", "newUserPassword", UserRole.Admin)
            };

            var content = new StringContent(JsonSerializer.Serialize(requestParam), Encoding.UTF8, "application/json");
            var result = await client.PostAsync(requestUri, content);
        }
    }
}
