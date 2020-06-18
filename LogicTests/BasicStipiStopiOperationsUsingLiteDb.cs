﻿using LogicTests.RepositoryHandling;

namespace LogicTests
{
    public class BasicStipiStopiOperationsUsingLiteDb : BasicStipiStopiOperations
    {
        public override ISsRepositoryImplementation SsRepositoryImplementation =>
            new LiteDbSsRepositoryImplementation();
    }
}