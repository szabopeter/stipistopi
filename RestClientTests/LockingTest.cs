using System.Linq;
using ServiceInterfaces.Dto;
using Xunit;

namespace RestClientTests
{
    public class LockingTest
    {
        [Fact]
        public async void Release_locks_on_User_deletion()
        {
            var testRestClient = new TestRestClient();
            var adminClient = testRestClient.RestClient;
            var res1 = (await adminClient.AddResource(new SsResource("resource", "address"))).Result;
            var user = (await adminClient.AddUser("user", "pass", UserRole.Regular)).Result;

            var userClient = testRestClient.GetAdditionalRestClient("user", "pass");
            var result = await userClient.LockOperation("lock", res1.ShortName);
            Assert.True(result.Success && result.Result);

            var resource = (await adminClient.GetResources()).Single();
            Assert.False(resource.IsAvailable);
            Assert.Equal("USER", resource.Locking.LockedBy.UserName);

            var delUserResult = await adminClient.DelUser("user");
            Assert.True(delUserResult.Success && delUserResult.Result);

            resource = (await adminClient.GetResources()).Single();
            Assert.True(resource.IsAvailable);
            Assert.Null(resource.Locking.LockedBy);

            var user2 = (await adminClient.AddUser("user2", "pass", UserRole.Regular)).Result;
            var user2Client = testRestClient.GetAdditionalRestClient("user2", "pass");
            result = await user2Client.LockOperation("lock", res1.ShortName);
            Assert.True(result.Success && result.Result);

            resource = (await adminClient.GetResources()).Single();
            Assert.False(resource.IsAvailable);
            Assert.Equal("USER2", resource.Locking.LockedBy.UserName);
        }
    }
}
