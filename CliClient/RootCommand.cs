using McMaster.Extensions.CommandLineUtils;

namespace CliClient
{
    [Command(
        Name = "CliClient",
        Description = "StipiStopi command line interface"),
        Subcommand(
            typeof(ResourcesCommand),
            typeof(AddUserCommand)
            )]
    public class RootCommand
    {
#pragma warning disable RCS1213, IDE0051 // Used by CLI parser
        private int OnExecute(CommandLineApplication app)
#pragma warning restore
        {
            app.ShowHint();
            return 0;
        }
    }
}