using LogicTests.RepositoryHandling;

namespace LogicTest
{
    public class BasicStipiStopiOperationsUsingInMemoryRepository : BasicStipiStopiOperations
    {
        public override ISsRepositoryImplementation SsRepositoryImplementation =>
            new InMemorySsRepositoryImplementation();
    }
}
