using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using McMaster.Extensions.CommandLineUtils;

namespace CliClient
{
    public class RestHttpClient : IRestHttpClient
    {
        public string BaseUri { get; }
        public HttpClient HttpClient { get; }
        public void WriteLine(string line)
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
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = NoCertificateValidation
                };
                HttpClient = new HttpClient(handler);
            }
            else
            {
                HttpClient = new HttpClient();
            }
        }

        private bool NoCertificateValidation(
            HttpRequestMessage request,
            X509Certificate2 cert,
            X509Chain chain,
            SslPolicyErrors errors
            ) => true;
    }
}
