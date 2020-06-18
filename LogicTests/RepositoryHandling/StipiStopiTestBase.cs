using logic;
using ServiceInterfaces.Dto;
using System;

namespace LogicTests.RepositoryHandling
{
    public abstract class StipiStopiTestBase<TSsRepositoryImplementation> : IDisposable where TSsRepositoryImplementation : ISsRepositoryImplementation
    {
        public StipiStopiWrapper<TSsRepositoryImplementation> StipiStopiWrapper { get; }
            = new StipiStopiWrapper<TSsRepositoryImplementation>();

        public StipiStopi Sut => StipiStopiWrapper.Service;

        public SsUser AdminUser => StipiStopiWrapper.AdminUser;

        public void Dispose()
        {
            StipiStopiWrapper.SsRepositoryImplementation.DisposeRepository();
        }
    }
}
