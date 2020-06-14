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
            var client = new RestClient("https://localhost:8140", "test", "test", true);
            var users = await client.GetUsers().ConfigureAwait(true);
            foreach (var user in users)
            {
                console.WriteLine($"{user.UserName,20} {user.Role,20}");
            }
        }
    }
}