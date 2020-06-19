namespace RestClient
{
    public class RestClientCommand<TRequest, TResponse>
    {
        public string Uri { get; }
        public TRequest RequestParam { get; }
        public RestClientCommand(string uri, TRequest requestParam)
        {
            Uri = uri;
            RequestParam = requestParam;
        }
    }
}