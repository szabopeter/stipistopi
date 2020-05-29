using logic;
using ServiceInterfaces;
using ServiceInterfaces.Dto;
using System;
using Xunit;

namespace LogicTest
{
    public abstract class StipiStopiTestBase
    {
        private StipiStopi sut;

        public StipiStopi Sut => sut ??= CreateSut();

        private StipiStopi CreateSut()
        {
            var repository = CreateRepository();
            repository.Transaction(() => repository.SaveUser(new SsUserSecret(AdminUser)));
            return new StipiStopi(repository);
        }

        protected abstract ISsRepository CreateRepository();

        public SsUser AdminUser { get; }

        [Fact]
        public void Empty_repository_Should_deliver_empty_resourcelist()
        {
            Assert.Empty(sut.GetResources());
        }

        [Fact]
        public void Create_Resource()
        {
            var regularUser = CreateUser();
            Assert.ThrowsAny<Exception>(() => sut.NewResource("NCU", "192.168.42.42", regularUser));
            var sameResource = sut.NewResource("NCU", "127.0.0.1", AdminUser);
            Assert.ThrowsAny<Exception>(() => sut.NewResource("NCU", "192.168.42.42", AdminUser));
            Assert.Single(sut.GetResources());
        }

        [Fact]
        public void Create_User()
        {
            var user = sut.NewUser("Alice", "pass", AdminUser);
            Assert.ThrowsAny<Exception>(() => sut.NewUser("Alice", "pass", AdminUser));
        }

        [Fact]
        public void Locked_resource_Should_not_be_locked_again()
        {
            var user = CreateUser("Bob");
            var res = sut.NewResource("Beles", "beles.local", AdminUser);
            Assert.True(sut.LockResource(res, user), "Initial lock should not fail but it did.");
            Assert.False(sut.LockResource(res, user), "Re-lock succeeded but it should not.");
            Assert.True(sut.IsLocked(res), "Resource should be locked but it is not");
        }

        [Fact]
        public void Locking_Should_require_valid_user()
        {
            var res = sut.NewResource("NCU1", "ncu1.local", AdminUser);
            var user = CreateUser("Bob");
            Assert.ThrowsAny<Exception>(() => sut.LockResource(res, new SsUser("Bob", "bad password")));
            Assert.True(sut.IsFree(res), "Resource should be free but it is not.");
        }

        [Fact]
        public void Released_resource_Should_be_free_and_lockable()
        {
            var userCharlie = CreateUser("Charlie");
            var res = sut.NewResource("NCU2", "ncu2.local", AdminUser);
            Assert.True(sut.LockResource(res, userCharlie), "Initial lock should succeed but it did not.");
            Assert.True(sut.IsLocked(res), "Resource should be locked but it is not");
            var userDavid = CreateUser("David");
            var releaseByOther = sut.ReleaseResource(res, userDavid);
            Assert.False(releaseByOther, "Release by other user should not succeed but it did.");
            var releaseSuccess = sut.ReleaseResource(res, userCharlie);
            Assert.True(releaseSuccess, "Release by the locking user should succeed but it did not.");
            Assert.True(sut.IsFree(res), "Resource should be free but it is not.");
            Assert.True(sut.LockResource(res, userCharlie), "Locking again should succeed but it did not.");
        }

        public SsUser CreateUser(string userName = "any", string password = "any")
        {
            return sut.NewUser(userName, password, AdminUser);
        }
    }
}
