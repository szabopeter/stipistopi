using LogicTests.RepositoryHandling;

namespace LogicTests
{
    public class LockingTestsUsingLiteDb : LockingTests
    {
        public override ISsRepositoryImplementation SsRepositoryImplementation =>
            new LiteDbSsRepositoryImplementation();
    }
}