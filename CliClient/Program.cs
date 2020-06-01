using System;
using System.Threading.Tasks;

namespace CliClient
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            // https://localhost:8140/stipistopi/resources
            var client = new RestClient("https://localhost:8140", "test", "test", true);
            var resources = await client.GetResources();
            foreach(var resource in resources)
            {
                Console.WriteLine($"{resource.ShortName, 20} {resource.Address, 20} {resource.LockedBy, 20}");
            }
        }
    }
}