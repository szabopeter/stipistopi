using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace CliClient
{
    [Command(Description = "Import database")]
    public class DbImportCommand
    {
#pragma warning disable RCS1213, IDE0051 // Used by CLI parser
        private async Task OnExecuteAsync(IConsole console)
#pragma warning restore
        {
            var client = Parent.CreateRestClient();
            var content = System.IO.File.ReadAllText(FileName, Encoding.UTF8);
            var result = await client.DbImport(content).ConfigureAwait(true);
            if (!result.Success)
            {
                console.WriteLine("Error: " + result.Error.Message);
                return;
            }

            console.WriteLine(result.Result
                ? "Import was successful"
                : $"Sorry, import from {FileName} failed!");
        }

        [Argument(0)]
        [Required]
        public string FileName { get; }

#pragma warning disable RCS1170
        private RootCommand Parent { get; set; }
#pragma warning restore
    }
}