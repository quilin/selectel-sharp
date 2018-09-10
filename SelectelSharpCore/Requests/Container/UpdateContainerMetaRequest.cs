using System.Net;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using SelectelSharpCore.Models.Container;
using SelectelSharpCore.Headers;

namespace SelectelSharpCore.Requests.Container
{
    /// <summary>
    /// Запрос на изменение мета-данных контейнера
    /// </summary>
    public class UpdateContainerMetaRequest : ContainerRequest<UpdateContainerResult>
    {
        /// <param name="containerName">Имя контейнера должно быть меньше 256 символов и не содержать завершающего слеша '/' в конце.</param>
        /// <param name="customHeaders">Произвольные мета-данные через передачу заголовков с префиксом X-Container-Meta-.</param>
        /// <param name="type">X-Container-Meta-Type: Тип контейнера (public, private, gallery)</param>
        /// <param name="corsHeaders"></param>
        public UpdateContainerMetaRequest(
            string containerName,
            ContainerType type = ContainerType.Private,
            IDictionary<string, object> customHeaders = null,
            CorsHeaders corsHeaders = null)
            : base(containerName)
        {
            customHeaders?.Add(HeaderKeys.XContainerMetaType, type.ToString().ToLower());
            SetCustomHeaders(customHeaders);
            SetCorsHeaders(corsHeaders);
        }

        internal override HttpMethod Method => HttpMethod.Put;

        internal override void Parse(HttpResponseHeaders headers, object data, HttpStatusCode status)
        {
            if (status == HttpStatusCode.Accepted)
            {
                Result = UpdateContainerResult.Updated;
            }
            else if (status == HttpStatusCode.Created)
            {
                Result = UpdateContainerResult.Created;
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
                Result = UpdateContainerResult.NotFound;
            }
            else
            {
                base.ParseError(null, status);
            }
        }
    }
}