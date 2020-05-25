using LiteDbSsRepositoryService;
using ServiceInterfaces.Dto;

namespace LogicTest
{
    public class LiteDbTests : StipiStopiTestBase
    {
        public LiteDbTests() : base(
            // TODO Use a memorystream
            admin => new LiteDbSsRepository(System.IO.Path.GetTempFileName()+".litedb"),
            new SsUser("testadmin", "testadmin", UserRole.Admin)
        )
        {
        }
    }
}
