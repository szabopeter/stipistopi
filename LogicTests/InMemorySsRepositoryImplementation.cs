using Logic.Repository;
using ServiceInterfaces;

namespace LogicTest
{
    public class InMemorySsRepositoryImplementation : ISsRepositoryImplementation
    {
        public ISsRepository InitializeRepository()
        {
            return new InMemorySsRepository();
        }

        public void DisposeRepository()
        {
        }
    }
}
