using Xunit;

namespace CliClientTests
{
    public class ResourcesTest
    {
        [Fact]
        public async void Initialization()
        {

            var restClient = new TestRestClient().RestClient;
            var resources = await restClient.GetResources();
            Assert.Empty(resources);
        }
    }
}
