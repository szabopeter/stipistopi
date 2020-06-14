using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace CliClient
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            await CommandLineApplication.ExecuteAsync<RootCommand>(args).ConfigureAwait(true);
        }
    }
}