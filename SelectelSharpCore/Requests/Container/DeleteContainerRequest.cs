using SelectelSharpCore.Models.Container;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SelectelSharpCore.Requests.Container
{
    /// <summary>
    /// Запрос на удаление контейнера
    /// </summary>
    public class DeleteContainerRequest : ContainerRequest<DeleteContainerResult>
    {
        public DeleteContainerRequest(string containerName)
            : base(containerName)
        {
        }

        internal override HttpMethod Method => HttpMethod.Delete;

        internal override void Parse(HttpResponseHeaders headers, object data, HttpStatusCode status)
        {
            if (status == HttpStatusCode.NoContent)
            {
                Result = DeleteContainerResult.Deleted;
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
                Result = DeleteContainerResult.NotFound;
            }
            else if (status == HttpStatusCode.Conflict)
            {
                Result = DeleteContainerResult.NotEmpty;
            }
            else
            {
                base.ParseError(ex, status);
            }
        }
    }
}