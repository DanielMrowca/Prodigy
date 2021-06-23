namespace HoneyComb.WebApi.ContentResults
{
    public class DefaultHttpResponseContentResultProvider : IHttpResponseContentResultProvider
    {
        public IHttpResponseContentResult GetResponseContentResult<TResult>(TResult result)
        {
            if (result is IFileResult fileResult)
                return new FileContentResult(fileResult.FileContents, fileResult.ContentType) 
                { 
                    EntityTag = fileResult.EntityTag,
                    FileDownloadName = fileResult.FileDownloadName,
                    LastModified = fileResult.LastModified
                };
            else
                return new JsonContentResult(result);
        }
    }
}
