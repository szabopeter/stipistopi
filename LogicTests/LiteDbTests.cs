using System;
using System.IO;
using LiteDbSsRepositoryService;
using ServiceInterfaces;

namespace LogicTest
{
    public class LiteDbTests : StipiStopiTestBase, IDisposable
    {
        override protected ISsRepository CreateRepository()
        {
            var zeroFile = Path.GetTempFileName();
            File.Delete(zeroFile);
            tempLiteDbFile = zeroFile + ".litedb";
            return new LiteDbSsRepository(tempLiteDbFile);
        }

        public void Dispose()
        {
            if (tempLiteDbFile == null)
                return;

            File.Delete(tempLiteDbFile);
            tempLiteDbFile = null;
        }

        private string tempLiteDbFile;
    }
}
