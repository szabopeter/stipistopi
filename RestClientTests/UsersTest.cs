using System;
using System.Linq;
using System.Threading.Tasks;
using ServiceInterfaces.Dto;
using Xunit;

namespace RestClientTests
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
            var testHost = new TestRestClient();
            var restClient = testHost.RestClient;
            async Task<int> CountUsers() => (await restClient.GetUsers()).Result.Count();
            Assert.Equal(1, await CountUsers());
            await restClient.AddUser("newUserName", "newUserPassword", UserRole.Admin);
            var result = await restClient.DelUser(testHost.Admin.UserName);
            Assert.True(result.Success && !result.Result);
            Assert.Equal(2, await CountUsers());

            result = await restClient.DelUser("newUserName");
            Assert.True(result.Success && result.Result);
            var userList = (await restClient.GetUsers()).Result;
            var remainingUser = userList.Single();
            Assert.Equal(testHost.Admin.UserName, remainingUser.UserName);
        }

        [Fact]
        public async void TestPasswordChange()
        {
            var testHost = new TestRestClient();
            var adminClient = testHost.RestClient;
            var user = (await adminClient.AddUser("bob", "pass1", UserRole.Regular)).Result;

            {
                var userClientWithPass1 = testHost.GetAdditionalRestClient(user.UserName, user.Password);
                {
                    var isPass1Valid = (await userClientWithPass1.GetUsers()).Success;
                    Assert.True(isPass1Valid);
                }
                {
                    var hasUserChangedAdminPassword = await userClientWithPass1.ChangePassword(testHost.Admin);
                    Assert.False(hasUserChangedAdminPassword.Success);
                }
                {
                    var hasAdminChangedUserPassword = await adminClient.ChangePassword(
                        new SsUser(user.UserName, "pass2"));
                    Assert.True(hasAdminChangedUserPassword.Success && hasAdminChangedUserPassword.Result);
                }
                {
                    var isPass1Valid = (await userClientWithPass1.GetUsers()).Success;
                    Assert.False(isPass1Valid);
                }
            }
            {
                var userClientWithPass2 = testHost.GetAdditionalRestClient(user.UserName, "pass2");
                {
                    var isPass2Valid = (await userClientWithPass2.GetUsers()).Success;
                    Assert.True(isPass2Valid);
                }
                {
                    var hasUserChangedOwnPassword = await userClientWithPass2.ChangePassword(
                        new SsUser(user.UserName, "pass3"));
                    Assert.True(hasUserChangedOwnPassword.Success && hasUserChangedOwnPassword.Result);
                }
                {
                    var isPass2Valid = (await userClientWithPass2.GetUsers()).Success;
                    Assert.False(isPass2Valid);
                }
            }
            {
                var userClientWithPass3 = testHost.GetAdditionalRestClient(user.UserName, "pass3");
                {
                    var isPass3Valid = (await userClientWithPass3.GetUsers()).Success;
                    Assert.True(isPass3Valid);
                }
            }
        }
    }
}
