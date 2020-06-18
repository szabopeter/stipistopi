using System.IO;
using LiteDbSsRepositoryService;
using ServiceInterfaces;

namespace LogicTest
{
    public class LiteDbSsRepositoryImplementation : ISsRepositoryImplementation
    {
        public ISsRepository InitializeRepository()
        {
            var zeroFile = Path.GetTempFileName();
            File.Delete(zeroFile);
            tempLiteDbFile = zeroFile + ".litedb";
            return new LiteDbSsRepository(tempLiteDbFile);
        }

        public void DisposeRepository()
        {
            if (tempLiteDbFile == null)
                return;

            File.Delete(tempLiteDbFile);
            tempLiteDbFile = null;
        }

        private string tempLiteDbFile;
    }
}
