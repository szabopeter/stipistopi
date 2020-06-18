using logic;
using ServiceInterfaces.Dto;
using System;

namespace LogicTests.RepositoryHandling
{
    public abstract class StipiStopiTestBase : IDisposable
    {
        public abstract ISsRepositoryImplementation SsRepositoryImplementation { get; }
        public StipiStopiWrapper StipiStopiWrapper => stipiStopiWrapper ??= new StipiStopiWrapper(SsRepositoryImplementation);

        public StipiStopi Sut => StipiStopiWrapper.Service;

        public SsUser AdminUser => StipiStopiWrapper.AdminUser;

        public SsUser CreateUser(string userName = "any", string password = "any")
        {
            // TODO: this should go to some common utility class, not in the base test class
            return Sut.NewUser(new SsUser(userName, password), AdminUser);
        }

        public void Dispose()
        {
            StipiStopiWrapper.SsRepositoryImplementation.DisposeRepository();
        }

        private StipiStopiWrapper stipiStopiWrapper;
    }
}
