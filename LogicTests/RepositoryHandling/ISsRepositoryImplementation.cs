using ServiceInterfaces;

namespace LogicTests.RepositoryHandling
{
    public interface ISsRepositoryImplementation
    {
        void DisposeRepository();
        ISsRepository InitializeRepository();
    }
}
