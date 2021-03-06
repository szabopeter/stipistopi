using McMaster.Extensions.CommandLineUtils;
using RestClient;

namespace CliClient
{
    [Command(
        Name = "CliClient",
        Description = "StipiStopi command line interface"),
        Subcommand(
            typeof(ResourcesCommand),
            typeof(AddUserCommand),
            typeof(UsersCommand),
            typeof(AddResourceCommand),
            typeof(DelResourceCommand),
            typeof(DelUserCommand),
            typeof(DbExportCommand),
            typeof(DbImportCommand)
            // TODO Lock + Release
            )]
    public class RootCommand
    {
        public RootCommand(IConsole console)
        {
            Console = console;
        }

#pragma warning disable RCS1213, IDE0051 // Used by CLI parser
        private int OnExecute(CommandLineApplication app)
#pragma warning restore
        {
            app.ShowHint();
            return 0;
        }

        public RestClient.RestClient CreateRestClient()
        {
            var restHttpClient = new RestHttpClient(BaseUrl ?? "https://localhost:8140", IgnoreServerCertificate, s => Console.WriteLine(s));
            var client = new RestClient.RestClient(restHttpClient, UserName ?? "test", Password ?? "test");
            return client;
        }

        [Option]
        public string UserName { get; }

        [Option]
        public string Password { get; }

        [Option]
        public string BaseUrl { get; }

        [Option]
        public bool IgnoreServerCertificate { get; }

#pragma warning disable RCS1170
        private IConsole Console { get; set; }
#pragma warning restore
    }
}