using SelectelSharpCore.Headers;
using SelectelSharpCore.Models.File;
using System.Net;
using System.Net.Http.Headers;

namespace SelectelSharpCore.Requests.File
{
    public class GetFileRequest : FileRequest<GetFileResult>
    {
        private bool allowAnonymously;

        public GetFileRequest(string containerName, string fileName, ConditionalHeaders conditionalHeaders = null,
            bool allowAnonymously = false)
            : base(containerName, fileName)
        {
            this.allowAnonymously = allowAnonymously;

            SetConditionalHeaders(conditionalHeaders);
        }

        public override bool AllowAnonymously => allowAnonymously;

        public override bool DownloadData => true;

        internal override void Parse(HttpResponseHeaders headers, object data, HttpStatusCode status)
        {
            if (status == HttpStatusCode.OK)
            {
                Result = new GetFileResult((byte[]) data, FileName, headers);
            }
            else
            {
                ParseError(null, status);
            }
        }
    }
}