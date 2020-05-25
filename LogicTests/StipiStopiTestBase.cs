using logic;
using ServiceInterfaces;
using ServiceInterfaces.Dto;
using System;
using Xunit;

namespace LogicTest
{

    public abstract class StipiStopiTestBase
    {
        public StipiStopi Sut { get; }
        public SsUser AdminUser { get; }

        public StipiStopiTestBase(Func<SsUser, ISsRepository> ssRepositoryFactory, SsUser adminUser)
        {
            AdminUser = adminUser;
            var repository = ssRepositoryFactory(adminUser);
            Sut = new StipiStopi(repository);
            repository.Transaction(() => repository.SaveUser(new SsUserSecret(adminUser)));
        }

        [Fact]
        public void Empty_repository_Should_deliver_empty_resourcelist()
        {
            Assert.Empty(Sut.GetResources());
        }

        [Fact]
        public void Create_Resource()
        {
            var regularUser = CreateUser();
           Assert.ThrowsAny<Exception>(() => Sut.NewResource("NCU", "192.168.42.42", regularUser));
            var sameResource = Sut.NewResource("NCU", "127.0.0.1", AdminUser);
            Assert.ThrowsAny<Exception>(() => Sut.NewResource("NCU", "192.168.42.42", AdminUser));
            Assert.Single(Sut.GetResources());
        }

        [Fact]
        public void Create_User()
        {
            var user = Sut.NewUser("Alice", "pass", AdminUser);
            Assert.ThrowsAny<Exception>(() => Sut.NewUser("Alice", "pass", AdminUser));
        }

        [Fact]
        public void Locked_resource_Should_not_be_locked_again()
        {
            var user = CreateUser("Bob");
            var res = Sut.NewResource("Beles", "beles.local", AdminUser);
            Assert.True(Sut.LockResource(res, user), "Initial lock should not fail but it did.");
            Assert.False(Sut.LockResource(res, user), "Re-lock succeeded but it should not.");
            Assert.True(Sut.IsLocked(res), "Resource should be locked but it is not");
        }

        [Fact]
        public void Locking_Should_require_valid_user()
        {
            var res = Sut.NewResource("NCU1", "ncu1.local", AdminUser);
            var user = CreateUser("Bob");
            Assert.ThrowsAny<Exception>(() => Sut.LockResource(res, new SsUser("Bob", "bad password")));
            Assert.True(Sut.IsFree(res), "Resource should be free but it is not.");
        }

        [Fact]
        public void Released_resource_Should_be_free_and_lockable()
        {
            var userCharlie = CreateUser("Charlie");
            var res = Sut.NewResource("NCU2", "ncu2.local", AdminUser);
            Assert.True(Sut.LockResource(res, userCharlie), "Initial lock should succeed but it did not.");
            Assert.True(Sut.IsLocked(res), "Resource should be locked but it is not");
            var userDavid = CreateUser("David");
            var releaseByOther = Sut.ReleaseResource(res, userDavid);
            Assert.False(releaseByOther, "Release by other user should not succeed but it did.");
            var releaseSuccess = Sut.ReleaseResource(res, userCharlie);
            Assert.True(releaseSuccess, "Release by the locking user should succeed but it did not.");
            Assert.True(Sut.IsFree(res), "Resource should be free but it is not.");
            Assert.True(Sut.LockResource(res, userCharlie), "Locking again should succeed but it did not.");
        }

        public SsUser CreateUser(string userName = "any", string password = "any")
        {
            return Sut.NewUser(userName, password, AdminUser);
        }
    }
}
