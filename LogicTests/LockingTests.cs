using LogicTests.RepositoryHandling;
using ServiceInterfaces.Dto;
using System;
using Xunit;

namespace LogicTests
{
    public abstract class LockingTests : StipiStopiTestBase
    {
        [Fact]
        public void Locked_resource_Should_not_be_locked_again()
        {
            var user = CreateUser("Bob");
            var res = Sut.NewResource(new SsResource("Beles", "beles.local"), AdminUser);
            Assert.True(Sut.LockResource(res.ShortName, user), "Initial lock should not fail but it did.");
            Assert.False(Sut.LockResource(res.ShortName, user), "Re-lock succeeded but it should not.");
            Assert.True(Sut.IsLocked(res), "Resource should be locked but it is not");
        }

        [Fact]
        public void Locking_Should_require_valid_user()
        {
            var res = Sut.NewResource(new SsResource("NCU1", "ncu1.local"), AdminUser);
            var user = CreateUser("Bob");
            Assert.ThrowsAny<Exception>(() => Sut.LockResource(res.ShortName, new SsUser("Bob", "bad password")));
            Assert.True(Sut.IsFree(res), "Resource should be free but it is not.");
        }

        [Fact]
        public void Released_resource_Should_be_free_and_lockable()
        {
            var userCharlie = CreateUser("Charlie");
            var res = Sut.NewResource(new SsResource("NCU2", "ncu2.local"), AdminUser);
            Assert.True(Sut.LockResource(res.ShortName, userCharlie), "Initial lock should succeed but it did not.");
            Assert.True(Sut.IsLocked(res), "Resource should be locked but it is not");
            var userDavid = CreateUser("David");
            var releaseByOther = Sut.ReleaseResource(res.ShortName, userDavid);
            Assert.False(releaseByOther, "Release by other user should not succeed but it did.");
            var releaseSuccess = Sut.ReleaseResource(res.ShortName, userCharlie);
            Assert.True(releaseSuccess, "Release by the locking user should succeed but it did not.");
            Assert.True(Sut.IsFree(res), "Resource should be free but it is not.");
            Assert.True(Sut.LockResource(res.ShortName, userCharlie), "Locking again should succeed but it did not.");
        }
    }
}
