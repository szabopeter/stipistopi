namespace LogicTests.CodeGeneration
{
    public class TestGenerationEntry
    {
        public TestGenerationEntry(string baseTest, string repositoryImplementation)
        {
            BaseTest = baseTest;
            RepositoryImplementation = repositoryImplementation;
        }

        public string BaseTest { get; }
        public string RepositoryImplementation { get; }
        public string Filename => $"{BaseTest}Using{RepositoryImplementation}.cs";
    }
}