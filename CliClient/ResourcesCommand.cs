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
            var client = Parent.CreateRestClient();
            var resources = await client.GetResources().ConfigureAwait(true);
            foreach (var resourceInfo in resources)
            {
                var resource = resourceInfo.Resource;
                console.WriteLine($"{resource.ShortName,20} {resource.Address,20} {resourceInfo.Locking.LockedBy?.UserName,20} {resourceInfo.Locking.LockedAt, 20}");
            }
        }

#pragma warning disable RCS1170
        private RootCommand Parent { get; set; }
#pragma warning restore
    }
}