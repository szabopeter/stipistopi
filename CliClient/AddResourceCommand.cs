using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using ServiceInterfaces.Dto;

namespace CliClient
{
    [Command(Description = "Add new resource")]
    public class AddResourceCommand
    {
#pragma warning disable RCS1213, IDE0051 // Used by CLI parser
        private async Task OnExecuteAsync(IConsole console)
#pragma warning restore
        {
            var client = Parent.CreateRestClient();
            var result = await client.AddResource(Resource).ConfigureAwait(true);
            if (result.Success)
                console.WriteLine("OK");
            else
                console.WriteLine("Error: " + result.Error.Message);
        }

        [Argument(0)]
        [Required]
        public string ShortName { get; }

        [Argument(1)]
        [Required]
        public string Address { get; }

        public SsResource Resource => new SsResource(ShortName, Address);

#pragma warning disable RCS1170
        private RootCommand Parent { get; set; }
#pragma warning restore
    }
}