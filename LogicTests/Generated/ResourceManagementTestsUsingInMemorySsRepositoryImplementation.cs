
using LogicTests.RepositoryHandling;

namespace LogicTests.Generated
{
    // This is generated code, modifications will be overwritten!
    public class ResourceManagementTestsUsingInMemorySsRepositoryImplementation : ResourceManagementTests
    {
        public override ISsRepositoryImplementation SsRepositoryImplementation =>
            new InMemorySsRepositoryImplementation();
    }
}
