using System;
using System.Net.Http;

namespace RestClient
{
    public interface IRestHttpClient
    {
        string BaseUri { get; }
        HttpClient HttpClient { get; }
        Action<string> WriteLine { get; }
    }
}
