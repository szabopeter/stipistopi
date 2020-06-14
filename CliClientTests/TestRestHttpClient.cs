using System.Collections.Generic;
using System.Net.Http;
using CliClient;

namespace CliClientTests
{
    class TestRestHttpClient : IRestHttpClient
    {
        public string BaseUri => "";

        public HttpClient HttpClient { get; }

        public void WriteLine(string line)
        {
            Lines.Add(line);
        }

        public List<string> Lines { get; } = new List<string>();

        public TestRestHttpClient(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }
    }
}
