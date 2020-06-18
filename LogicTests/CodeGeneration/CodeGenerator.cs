using System.IO;
using LogicTests.RepositoryHandling;
using Microsoft.CodeAnalysis.CSharp;

namespace LogicTests.CodeGeneration
{
    public class CodeGenerator
    {
        public CodeGenerator(string path)
        {
            PathRoot = path;
            Directory.CreateDirectory(PathRoot);
        }

        public string PathRoot { get; }

        public string Template = @"
using LogicTests.RepositoryHandling;

namespace LogicTests.Generated
{
    // This is generated code, modifications will be overwritten!
    public class {{BaseTest}}Using{{RepositoryImplementation}} : {{BaseTest}}
    {
        public override ISsRepositoryImplementation SsRepositoryImplementation =>
            new {{RepositoryImplementation}}();
    }
}
";
        public bool EnsureUpdatedSources()
        {
            bool wasUpToDate = true;
            var entries = new[] {
                new TestGenerationEntry(nameof(LockingTests), nameof(InMemorySsRepositoryImplementation)),
                new TestGenerationEntry(nameof(LockingTests), nameof(LiteDbSsRepositoryImplementation)),
                new TestGenerationEntry(nameof(BasicStipiStopiOperations), nameof(InMemorySsRepositoryImplementation)),
                new TestGenerationEntry(nameof(BasicStipiStopiOperations), nameof(LiteDbSsRepositoryImplementation)),
            };
            foreach (var entry in entries)
            {
                var code = Template
                    .Replace("{{BaseTest}}", entry.BaseTest)
                    .Replace("{{RepositoryImplementation}}", entry.RepositoryImplementation);
                var syntax = CSharpSyntaxTree.ParseText(code);
                code = syntax.ToString();
                var targetFilename = Path.Combine(PathRoot, entry.Filename);
                var targetContent = File.Exists(targetFilename)
                    ? File.ReadAllText(targetFilename)
                    : "";
                if (targetContent == code)
                    continue;

                File.WriteAllText(targetFilename, code);
                wasUpToDate = false;
            }
            return wasUpToDate;
        }
    }
}