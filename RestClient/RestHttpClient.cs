using System;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace RestClient
{
    public class RestHttpClient : IRestHttpClient
    {
        public string BaseUri { get; }
        public HttpClient HttpClient { get; }
        public Action<string> WriteLine { get; }

        public RestHttpClient(string baseUri, bool ignoreServerCertificate, Action<string> consoleWriteLine)
        {
            BaseUri = baseUri;
            WriteLine = consoleWriteLine;
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
