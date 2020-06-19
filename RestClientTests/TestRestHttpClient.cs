using System;
using System.Collections.Generic;
using System.Net.Http;
using RestClient;

namespace RestClientTests
{
    class TestRestHttpClient : IRestHttpClient
    {
        public string BaseUri => "";

        public HttpClient HttpClient { get; }

        public Action<string> WriteLine { get; }

        public List<string> Lines { get; } = new List<string>();

        public TestRestHttpClient(HttpClient httpClient)
        {
            HttpClient = httpClient;
            WriteLine = line => Lines.Add(line);
        }
    }
}
