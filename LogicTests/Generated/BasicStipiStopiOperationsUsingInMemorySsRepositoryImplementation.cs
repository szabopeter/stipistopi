
using LogicTests.RepositoryHandling;

namespace LogicTests.Generated
{
    // This is generated code, modifications will be overwritten!
    public class BasicStipiStopiOperationsUsingInMemorySsRepositoryImplementation : BasicStipiStopiOperations
    {
        public override ISsRepositoryImplementation SsRepositoryImplementation =>
            new InMemorySsRepositoryImplementation();
    }
}
