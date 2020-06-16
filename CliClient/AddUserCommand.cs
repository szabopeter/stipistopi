using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using ServiceInterfaces.Dto;

namespace CliClient
{
    [Command(Description = "Add new user")]
    public class AddUserCommand
    {
#pragma warning disable RCS1213, IDE0051 // Used by CLI parser
        private async Task OnExecuteAsync(IConsole console)
#pragma warning restore
        {
            if (Password == null)
            {
                Password = new Random().Next(100000000, 999999999).ToString();
                console.WriteLine($"Using generated password {Password}");
            }

            var client = Parent.CreateRestClient();
            var result = await client.AddUser(UserName, Password, Role).ConfigureAwait(true);
            if (result.Success)
                console.WriteLine("OK");
            else
                console.WriteLine("Error: " + result.Error.Message);
        }

        [Argument(0)]
        [Required]
        public string UserName { get; }

        [Option]
        public string Password { get; set; }

        [Option]
        public UserRole Role { get; }

#pragma warning disable RCS1170
        private RootCommand Parent { get; set; }
#pragma warning restore
    }
}