
using LogicTests.RepositoryHandling;

namespace LogicTests.Generated
{
    // This is generated code, modifications will be overwritten!
    public class UserManagementTestsUsingLiteDbSsRepositoryImplementation : UserManagementTests
    {
        public override ISsRepositoryImplementation SsRepositoryImplementation =>
            new LiteDbSsRepositoryImplementation();
    }
}
