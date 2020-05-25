using Logic.Repository;
using ServiceInterfaces.Dto;

namespace LogicTest
{
    public class InMemoryTests : StipiStopiTestBase
    {
        public InMemoryTests() : base(
            admin => new InMemorySsRepository(),
            new SsUser("testadmin", "testadmin", UserRole.Admin)
        )
        {
        }
    }
}
