using LiteDbSsRepositoryService;
using ServiceInterfaces;

namespace LogicTest
{
    public class LiteDbTests : StipiStopiTestBase
    {
        override protected ISsRepository CreateRepository()
        {
            // TODO Remove temp file on Dispose
            return new LiteDbSsRepository(System.IO.Path.GetTempFileName()+".litedb");
        }
    }
}
