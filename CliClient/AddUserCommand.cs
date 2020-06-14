using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace CliClient
{
    [Command(Description = "Add new user")]
    public class AddUserCommand
    {
#pragma warning disable RCS1213, IDE0051 // Used by CLI parser
        private async Task OnExecuteAsync(IConsole console)
#pragma warning restore
        {
            await Task.Run(() =>
            {
                if (Password == null)
                {
                    Password = new Random().Next(100000000, 999999999).ToString();
                }
                console.WriteLine($"TODO: create user with name {UserName} and password {Password}");
            }
            ).ConfigureAwait(true);
        }

        [Argument(0)]
        [Required]
        public string UserName { get; }

        [Argument(1)]
        public string Password { get; set; }
    }
}