using SelectelSharpCore.Models.Container;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SelectelSharpCore.Requests.Container
{
    public class GetContainerInfoRequest : ContainerRequest<ContainerInfo>
    {
        public GetContainerInfoRequest(string containerName) : base(containerName)
        {
        }

        internal override HttpMethod Method => HttpMethod.Head;

        internal override void Parse(HttpResponseHeaders headers, object data, HttpStatusCode status)
        {
            Result = new ContainerInfo(headers);
        }

        internal override void ParseError(HttpRequestException ex, HttpStatusCode status)
        {
            if (status == HttpStatusCode.NotFound)
            {
                Result = null;
            }
            else
            {
                base.ParseError(ex, status);
            }
        }
    }
}