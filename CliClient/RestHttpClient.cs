using System.Net.Http;
using McMaster.Extensions.CommandLineUtils;

namespace CliClient
{
    public class RestHttpClient : IRestHttpClient
    {
        public string BaseUri { get; }
        public HttpClient HttpClient { get; }
        public void WriteLine (string line)
        {
            Console.WriteLine(line);
        }

        public IConsole Console;

        public RestHttpClient(string baseUri, bool ignoreServerCertificate, IConsole console)
        {
            BaseUri = baseUri;
            Console = console;
            if (ignoreServerCertificate)
            {
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (request, cert, chain, errors) => true;
                HttpClient = new HttpClient(handler);
            }
            else
            {
                HttpClient = new HttpClient();
            }
        }
    }
}
