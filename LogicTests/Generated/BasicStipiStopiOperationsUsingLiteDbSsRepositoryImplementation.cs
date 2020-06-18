
using LogicTests.RepositoryHandling;

namespace LogicTests.Generated
{
    // This is generated code, modifications will be overwritten!
    public class BasicStipiStopiOperationsUsingLiteDbSsRepositoryImplementation : BasicStipiStopiOperations
    {
        public override ISsRepositoryImplementation SsRepositoryImplementation =>
            new LiteDbSsRepositoryImplementation();
    }
}
