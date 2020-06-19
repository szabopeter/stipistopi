using LogicTests.RepositoryHandling;
using ServiceInterfaces.Dto;
using ServiceInterfaces.Exceptions;
using System;
using System.Linq;
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
            Assert.ThrowsAny<InsufficientRoleException>(() => Sut.NewResource(new SsResource("NCU", "192.168.42.42"), regularUser));
            Sut.NewResource(new SsResource("NCU", "127.0.0.1"), AdminUser);
            Assert.ThrowsAny<ResourceAlreadyExistsException>(() => Sut.NewResource(new SsResource("NCU", "127.0.0.1"), AdminUser));
            Assert.Single(Sut.GetResources());
        }

        [Fact]
        public void Add_Description()
        {
            var regularUser = CreateUser();
            var resource = Sut.NewResource(new SsResource("my host", "192.168.42.1"), AdminUser);
            Assert.Equal("", Reget(resource).Description);

            var success = Sut.UpdateResourceDescription(resource, "Resource description", regularUser);
            Assert.True(success);
            Assert.Equal("Resource description", Reget(resource).Description);

            var staleResource = resource;
            success = Sut.UpdateResourceDescription(staleResource, "Description #2", regularUser);
            Assert.False(success);
            Assert.Equal("Resource description", Reget(resource).Description);
        }

        private SsResource Reget(SsResource resource)
        {
            return Sut.GetResources()
                .Where(res => res.ShortName.Equals(resource.ShortName))
                .Single();
        }
    }
}
