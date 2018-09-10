using SelectelSharpCore.Headers;
using SelectelSharpCore.Models.File;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SelectelSharpCore.Requests.File
{
    public class UpdateFileMetaRequest : FileRequest<UpdateFileResult>
    {
        internal override HttpMethod Method => HttpMethod.Post;

        public UpdateFileMetaRequest(
            string containerName,
            string fileName,
            IDictionary<string, object> customHeaders = null,
            CorsHeaders corsHeaders = null)
            : base(containerName, fileName)
        {
            SetCustomHeaders(customHeaders);
            SetCorsHeaders(corsHeaders);
        }

        internal override void Parse(HttpResponseHeaders headers, object data, HttpStatusCode status)
        {
            if (status == HttpStatusCode.NoContent)
            {
                Result = UpdateFileResult.Updated;
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
                Result = UpdateFileResult.NotFound;
            }
            else
            {
                base.ParseError(null, status);
            }
        }
    }
}