using System;
using System.Linq;
using CliClient;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using RestApi;
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
            var httpClient = testServer.CreateClient();
            var restHttpClient = new TestRestHttpClient(httpClient);
                // TODO There should be a common source for default admin credentials used here and in Populate 
            var restClient = new RestClient(restHttpClient, "test", "test");

            await restClient.AddUser("newUserName", "newUserPassword", UserRole.Admin);
            
            var userList = await restClient.GetUsers();
            var newUser = userList.Single(u => 
                string.Equals(u.UserName, "newUserName", StringComparison.InvariantCultureIgnoreCase));
            Assert.Equal(UserRole.Admin, newUser.Role);
        }
    }
}
