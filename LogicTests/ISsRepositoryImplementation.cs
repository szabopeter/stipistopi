using ServiceInterfaces;

namespace LogicTest
{
    public interface ISsRepositoryImplementation
    {
        void DisposeRepository();
        ISsRepository InitializeRepository();
    }
}
