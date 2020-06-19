using LogicTests.RepositoryHandling;
using ServiceInterfaces.Dto;
using System;
using Xunit;

namespace LogicTests
{
    public abstract class UserManagementTests : StipiStopiTestBase
    {
        [Fact]
        public void Create_User()
        {
            var user = Sut.NewUser(new SsUser("Alice", "pass"), AdminUser);
            Assert.ThrowsAny<Exception>(() => Sut.NewUser(user, AdminUser));
        }
    }
}
