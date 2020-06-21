using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace CliClient
{
    [Command(Description = "Export database")]
    public class DbExportCommand
    {
#pragma warning disable RCS1213, IDE0051 // Used by CLI parser
        private async Task OnExecuteAsync(IConsole console)
#pragma warning restore
        {
            var client = Parent.CreateRestClient();
            var result = await client.DbExport().ConfigureAwait(true);
            if (!result.Success)
            {
                console.WriteLine("Error: " + result.Error.Message);
                return;
            }

            System.IO.File.WriteAllText(FileName, result.Result, Encoding.UTF8);
            console.WriteLine($"Exported to {FileName}");
        }

        [Argument(0)]
        [Required]
        public string FileName { get; }

#pragma warning disable RCS1170
        private RootCommand Parent { get; set; }
#pragma warning restore
    }
}