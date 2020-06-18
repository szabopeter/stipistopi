using logic;
using ServiceInterfaces;
using ServiceInterfaces.Dto;
using System;

namespace LogicTests.RepositoryHandling
{
    public class StipiStopiWrapper
    {
        public StipiStopi Service => service ??= CreateSut();

        public SsUser AdminUser { get; } = new SsUser("testadmin", "testpassword", UserRole.Admin);

        public ISsRepositoryImplementation SsRepositoryImplementation { get; }

        public StipiStopiWrapper(ISsRepositoryImplementation ssRepositoryImplementation)
        {
            SsRepositoryImplementation = ssRepositoryImplementation;
        }

        private StipiStopi CreateSut()
        {
            var repository = SsRepositoryImplementation.InitializeRepository();
            repository.Transaction(() => repository.SaveUser(new SsUserSecret(AdminUser)));
            return new StipiStopi(repository);
        }

        private StipiStopi service;
    }
}
