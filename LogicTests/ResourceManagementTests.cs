using LogicTests.RepositoryHandling;
using ServiceInterfaces.Dto;
using System;
using Xunit;

namespace LogicTests
{
    public abstract class ResourceManagementTests : StipiStopiTestBase
    {
        [Fact]
        public void Empty_repository_Should_deliver_empty_resourcelist()
        {
            Assert.Empty(Sut.GetResources());
        }

        [Fact]
        public void Create_Resource()
        {
            var regularUser = CreateUser();
            Assert.ThrowsAny<Exception>(() => Sut.NewResource(new SsResource("NCU", "192.168.42.42"), regularUser));
            Sut.NewResource(new SsResource("NCU", "127.0.0.1"), AdminUser);
            Assert.ThrowsAny<Exception>(() => Sut.NewResource(new SsResource("NCU", "127.0.0.1"), AdminUser));
            Assert.Single(Sut.GetResources());
        }
    }
}
