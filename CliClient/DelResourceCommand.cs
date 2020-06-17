using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace CliClient
{
    [Command(Description = "Delete resource")]
    public class DelResourceCommand
    {
#pragma warning disable RCS1213, IDE0051 // Used by CLI parser
        private async Task OnExecuteAsync(IConsole console)
#pragma warning restore
        {
            var client = Parent.CreateRestClient();
            var result = await client.DelResource(ResourceName).ConfigureAwait(true);
            if (result.Success)
                console.WriteLine(result.Result ? "OK" : "Could not delete the resource!");
            else
                console.WriteLine("Error: " + result.Error.Message);
        }

        [Argument(0)]
        [Required]
        public string ResourceName { get; }

#pragma warning disable RCS1170
        private RootCommand Parent { get; set; }
#pragma warning restore
    }
}