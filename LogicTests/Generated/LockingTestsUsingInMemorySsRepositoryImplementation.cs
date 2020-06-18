
using LogicTests.RepositoryHandling;

namespace LogicTests.Generated
{
    // This is generated code, modifications will be overwritten!
    public class LockingTestsUsingInMemorySsRepositoryImplementation : LockingTests
    {
        public override ISsRepositoryImplementation SsRepositoryImplementation =>
            new InMemorySsRepositoryImplementation();
    }
}
