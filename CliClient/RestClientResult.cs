using System;
using RestApi;

namespace CliClient
{
    public class RestClientResult<TResponse>
        {
            public bool Success { get; }
            public TResponse Result => Success ? result : throw new NullReferenceException();
            public RestError Error => !Success ? error : throw new NullReferenceException();
            private readonly TResponse result;
            private readonly RestError error;
            public RestClientResult(TResponse response)
            {
                Success = true;
                result = response;
            }

            public RestClientResult(RestError error)
            {
                Success = false;
                this.error = error;
            }
        }
}