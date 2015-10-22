using System.Net;

namespace ApplicationUpdater
{
    internal class Response : IResponse
    {
        public HttpStatusCode StatusCode { get; private set; }
        public string ErrorMessage { get; private set; }
        public bool IsSuccess { get; private set; }

        public Response(HttpStatusCode statusCode, string errorMessage, bool isSuccess = false)
        {
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
            IsSuccess = isSuccess;
        }
    }
}