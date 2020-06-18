using Logic.Repository;
using ServiceInterfaces;

namespace LogicTests.RepositoryHandling
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
