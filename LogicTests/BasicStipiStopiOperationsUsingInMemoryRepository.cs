﻿using LogicTests.RepositoryHandling;

namespace LogicTests
{
    public class BasicStipiStopiOperationsUsingInMemoryRepository : BasicStipiStopiOperations
    {
        public override ISsRepositoryImplementation SsRepositoryImplementation =>
            new InMemorySsRepositoryImplementation();
    }
}
