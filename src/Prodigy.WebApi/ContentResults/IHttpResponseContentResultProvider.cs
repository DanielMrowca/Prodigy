namespace Prodigy.WebApi.ContentResults
{
    public interface IHttpResponseContentResultProvider
    {
        IHttpResponseContentResult GetResponseContentResult<TResult>(TResult result);
    }
}
