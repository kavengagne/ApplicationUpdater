using System.Net;

namespace ApplicationUpdater
{
    public interface IResponse
    {
        HttpStatusCode StatusCode { get; }
        string ErrorMessage { get; }
        bool IsSuccess { get; }
    }
}