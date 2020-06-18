using Logic.Repository;
using LogicTests.RepositoryHandling;
using ServiceInterfaces;

namespace LogicTest.RepositoryHandling
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
