using logic;
using ServiceInterfaces;
using ServiceInterfaces.Dto;
using System;

namespace LogicTest
{
    public class StipiStopiWrapper<TSsRepositoryImplementation> where TSsRepositoryImplementation : ISsRepositoryImplementation
    {
        public StipiStopi Service => service ??= CreateSut();

        public SsUser AdminUser { get; } = new SsUser("testadmin", "testpassword", UserRole.Admin);

        public ISsRepositoryImplementation SsRepositoryImplementation { get; }

        public StipiStopiWrapper()
        {
            SsRepositoryImplementation = (ISsRepositoryImplementation)Activator.CreateInstance(typeof(TSsRepositoryImplementation));
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
