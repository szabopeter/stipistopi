using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace CliClient
{
    [Command(Description = "List resources")]
    public class ResourcesCommand
    {
#pragma warning disable RCS1213, IDE0051 // Used by CLI parser
        private async Task OnExecuteAsync(IConsole console)
#pragma warning restore
        {
            var client = new RestClient("https://localhost:8140", "test", "test", true);
            var resources = await client.GetResources().ConfigureAwait(true);
            foreach (var resource in resources)
            {
                console.WriteLine($"{resource.ShortName,20} {resource.Address,20} {resource.LockedBy,20}");
            }
        }
    }
}