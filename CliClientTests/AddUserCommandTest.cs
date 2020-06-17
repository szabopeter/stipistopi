using System;
using System.Linq;
using ServiceInterfaces.Dto;
using Xunit;

namespace CliClientTests
{
    public class AddUserCommandTest
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
    }
}
