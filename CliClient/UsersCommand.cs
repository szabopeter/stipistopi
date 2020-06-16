using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace CliClient
{
    [Command(Description = "List users")]
    public class UsersCommand
    {
#pragma warning disable RCS1213, IDE0051 // Used by CLI parser
        private async Task OnExecuteAsync(IConsole console)
#pragma warning restore
        {
            var client = Parent.CreateRestClient();
            var result = await client.GetUsers().ConfigureAwait(true);
            if (!result.Success)
                console.WriteLine("Error: " + result.Error.Message);
            else
            {
                foreach (var user in result.Result)
                {
                    console.WriteLine($"{user.UserName,20} {user.Role,20}");
                }
            }
        }

#pragma warning disable RCS1170
        private RootCommand Parent { get; set; }
#pragma warning restore
    }
}