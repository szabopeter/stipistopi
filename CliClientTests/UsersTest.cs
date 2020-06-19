using System;
using System.Linq;
using System.Threading.Tasks;
using ServiceInterfaces.Dto;
using Xunit;

namespace CliClientTests
{
    public class UsersTest
    {
        [Fact]
        public async void TestAddAndList()
        {
            var testHost = new TestRestClient();
            var restClient = testHost.RestClient;
            await restClient.AddUser("newUserName", "newUserPassword", UserRole.Admin);
            var userList = await restClient.GetUsers();
            var newUser = userList.Result.Single(u =>
                string.Equals(u.UserName, "newUserName", StringComparison.InvariantCultureIgnoreCase));
            Assert.Equal(UserRole.Admin, newUser.Role);
        }

        [Fact]
        public async void TestRemoval()
        {
            // TODO test releasing resources
            var testHost = new TestRestClient();
            var restClient = testHost.RestClient;
            async Task<int> CountUsers() => (await restClient.GetUsers()).Result.Count();
            Assert.Equal(1, await CountUsers());
            await restClient.AddUser("newUserName", "newUserPassword", UserRole.Admin);
            Assert.Equal(2, await CountUsers());

            var result = await restClient.DelUser("newUserName");
            Assert.True(result.Success && result.Result);
            var userList = (await restClient.GetUsers()).Result;
            var remainingUser = userList.Single();
            Assert.Equal(testHost.Admin.UserName, remainingUser.UserName);
        }
    }
}
