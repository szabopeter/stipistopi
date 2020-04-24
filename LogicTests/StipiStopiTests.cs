using logic;
using System;
using Xunit;

namespace LogicTest
{
    public class StipiStopiTests
    {
        [Fact]
        public void EmptyRepositoryShouldDeliverEmptyResourceList()
        {
            var ss = new StipiStopi(new InMemorySsRepository());
            Assert.Empty(ss.GetResources());
        }

        [Fact]
        public void CreateResource()
        {
            var ss = new StipiStopi(new InMemorySsRepository());
            ss.NewResource(new SsResource());
            Assert.Single(ss.GetResources());
        }
    }
}
