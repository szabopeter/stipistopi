using LogicTests.RepositoryHandling;

namespace LogicTest
{
    public class BasicStipiStopiOperationsUsingLiteDb : BasicStipiStopiOperations
    {
        public override ISsRepositoryImplementation SsRepositoryImplementation =>
            new LiteDbSsRepositoryImplementation();
    }
}