using SelectelSharpCore.Headers;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SelectelSharpCore.Requests.File
{
    public class GetUploadArchiveStatusRequest :
        BaseRequest<string>
    {
        private string ExtractId { get; set; }

        internal override HttpMethod Method => HttpMethod.Get;


        public GetUploadArchiveStatusRequest(string extractId)
        {
            TryAddHeader(HeaderKeys.Accept, HeaderKeys.AcceptJson);
            ExtractId = extractId;
        }

        protected override string GetUrl(string storageUrl)
        {
            return string.Format("https://api.selcdn.ru/v1/extract-archive/{0}", ExtractId);
        }

        internal override void Parse(HttpResponseHeaders headers, object content, HttpStatusCode status)
        {
            base.Parse(headers, content, status);
        }
    }
}