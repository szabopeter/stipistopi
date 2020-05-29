using Logic.Repository;
using ServiceInterfaces;

namespace LogicTest
{
    public class InMemoryTests : StipiStopiTestBase
    {
        protected override ISsRepository CreateRepository()
        {
            return new InMemorySsRepository();
        }
    }
}
