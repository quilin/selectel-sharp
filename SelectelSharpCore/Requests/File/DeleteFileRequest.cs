using SelectelSharpCore.Models.File;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SelectelSharpCore.Requests.File
{
    public class DeleteFileRequest : FileRequest<DeleteFileResult>
    {
        public DeleteFileRequest(string containerName, string fileName)
            : base(containerName, fileName)
        {
        }

        internal override HttpMethod Method => HttpMethod.Delete;

        internal override void Parse(HttpResponseHeaders headers, object data, HttpStatusCode status)
        {
            if (status == HttpStatusCode.NoContent)
            {
                Result = DeleteFileResult.Deleted;
            }
            else
            {
                base.ParseError(null, status);
            }
        }

        internal override void ParseError(HttpRequestException ex, HttpStatusCode status)
        {
            if (status == HttpStatusCode.NotFound)
            {
                Result = DeleteFileResult.NotFound;
            }
            else
            {
                base.ParseError(ex, status);
            }
        }
    }
}