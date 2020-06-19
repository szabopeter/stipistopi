using System.Collections.Generic;
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
                new TestGenerationEntry(nameof(UserManagementTests), nameof(InMemorySsRepositoryImplementation)),
                new TestGenerationEntry(nameof(UserManagementTests), nameof(LiteDbSsRepositoryImplementation)),
                new TestGenerationEntry(nameof(ResourceManagementTests), nameof(InMemorySsRepositoryImplementation)),
                new TestGenerationEntry(nameof(ResourceManagementTests), nameof(LiteDbSsRepositoryImplementation)),
            };

            var filesToDelete = new HashSet<string>(Directory.GetFiles(PathRoot));
            foreach (var entry in entries)
            {
                var code = Template
                    .Replace("{{BaseTest}}", entry.BaseTest)
                    .Replace("{{RepositoryImplementation}}", entry.RepositoryImplementation);
                var syntax = CSharpSyntaxTree.ParseText(code);
                code = syntax.ToString();
                var targetFilename = Path.Combine(PathRoot, entry.Filename);
                var targetContent = "";
                if (File.Exists(targetFilename))
                {
                    targetContent = File.ReadAllText(targetFilename);
                    filesToDelete.Remove(targetFilename);
                }
                if (targetContent == code)
                    continue;

                File.WriteAllText(targetFilename, code);
                wasUpToDate = false;
            }
            foreach(var filename in filesToDelete)
                File.Delete(filename);
            wasUpToDate = wasUpToDate && filesToDelete.Count == 0;
            return wasUpToDate;
        }
    }
}