using LogicTests.RepositoryHandling;

namespace LogicTests
{
    public class LockingTestsUsingInMemoryRepository : LockingTests
    {
        public override ISsRepositoryImplementation SsRepositoryImplementation =>
            new InMemorySsRepositoryImplementation();
    }
}
