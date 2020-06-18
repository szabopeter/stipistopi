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

        public void Dispose()
        {
            StipiStopiWrapper.SsRepositoryImplementation.DisposeRepository();
        }

        private StipiStopiWrapper stipiStopiWrapper;
    }
}
