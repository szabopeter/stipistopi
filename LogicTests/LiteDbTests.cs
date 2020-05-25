using LiteDbSsRepositoryService;
using ServiceInterfaces.Dto;

namespace LogicTest
{
    public class LiteDbTests : StipiStopiTestBase
    {
        public LiteDbTests() : base(
            admin => new LiteDbSsRepository(),
            new SsUser("testadmin", "testadmin", UserRole.Admin)
        )
        {
        }
    }
}
