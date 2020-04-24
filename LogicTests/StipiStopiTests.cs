using logic;
using System;
using Xunit;

namespace LogicTest
{
    public class StipiStopiTests
    {
        public StipiStopi Sut = new StipiStopi(new InMemorySsRepository());

        [Fact]
        public void Empty_repository_Should_deliver_empty_resourcelist()
        {
            Assert.Empty(Sut.GetResources());
        }

        [Fact]
        public void Create_Resource()
        {
            var sameResource = new SsResource();
            Sut.NewResource(sameResource);
            Assert.ThrowsAny<Exception>(() => Sut.NewResource(sameResource));
            Assert.Single(Sut.GetResources());
        }

        [Fact]
        public void Create_User()
        {
            var user = new SsUser();
            Sut.NewUser(user);
            Assert.ThrowsAny<Exception>(() => Sut.NewUser(user));
        }

        [Fact]
        public void Locked_resource_Should_not_be_locked_again()
        {
            var user = new SsUser();
            Sut.NewUser(user);
            var res = new SsResource();
            Sut.NewResource(res);
            Assert.True(Sut.LockResource(res, user), "Initial lock should not fail but it did.");
            Assert.False(Sut.LockResource(res, user), "Re-lock succeeded but it should not.");
            Assert.True(Sut.IsLocked(res), "Resource should be locked but it is not");
        }

        [Fact]
        public void Locking_Should_require_valid_user()
        {
            var res = new SsResource();
            Assert.ThrowsAny<Exception>(() => Sut.LockResource(res, new SsUser()));
            Assert.True(Sut.IsFree(res), "Resource should be free but it is not.");
        }

        [Fact]
        public void Released_resource_Should_be_free_and_lockable()
        {
            var user = new SsUser();
            Sut.NewUser(user);
            var res = new SsResource();
            Sut.NewResource(res);
            Assert.True(Sut.LockResource(res, user), "Initial lock should succeed but it did not.");
            Assert.True(Sut.IsLocked(res), "Resource should be locked but it is not");
            var releaseByOther = Sut.ReleaseResource(res, new SsUser());
            Assert.False(releaseByOther, "Release by other user should not succeedd but it did.");
            var releaseSuccess = Sut.ReleaseResource(res, user);
            Assert.False(releaseSuccess, "Release by the locking user should succeedd but it did not.");
            Assert.True(Sut.IsFree(res), "Resource should be free but it is not.");
            Assert.True(Sut.LockResource(res, user), "Locking again should succeed but it did not.");
        }
    }
}
