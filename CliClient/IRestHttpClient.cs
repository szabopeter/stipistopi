using System.Net.Http;

namespace CliClient
{
    public interface IRestHttpClient
    {
        string BaseUri { get; }
        HttpClient HttpClient { get; }
        void WriteLine(string line);
    }
}
