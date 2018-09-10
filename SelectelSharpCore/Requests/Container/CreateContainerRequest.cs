using SelectelSharpCore.Headers;
using SelectelSharpCore.Models.Container;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SelectelSharpCore.Requests.Container
{
    /// <summary>
    /// Запрос на создание контейнера
    /// </summary>
    public class CreateContainerRequest : ContainerRequest<CreateContainerResult>
    {
        /// <param name="containerName">Имя контейнера должно быть меньше 256 символов и не содержать завершающего слеша '/' в конце.</param>
        /// <param name="customHeaders">Произвольные мета-данные через передачу заголовков с префиксом X-Container-Meta-.</param>
        /// <param name="type">X-Container-Meta-Type: Тип контейнера (public, private, gallery)</param>
        /// <param name="corsHeaders">Дополнительные заголовки кэшировани и CORS</param>
        public CreateContainerRequest(
            string containerName,
            ContainerType type = ContainerType.Private,
            Dictionary<string, object> customHeaders = null,
            CorsHeaders corsHeaders = null)
            : base(containerName)
        {
            if (customHeaders == null)
            {
                customHeaders = new Dictionary<string, object>();
            }

            customHeaders.Add(HeaderKeys.XContainerMetaType, type.ToString().ToLower());

            SetCustomHeaders(customHeaders);
            SetCorsHeaders(corsHeaders);
        }

        internal override HttpMethod Method => HttpMethod.Put;

        internal override void Parse(HttpResponseHeaders headers, object data, HttpStatusCode status)
        {
            if (status == HttpStatusCode.Created)
            {
                Result = CreateContainerResult.Created;
            }
            else if (status == HttpStatusCode.Accepted)
            {
                Result = CreateContainerResult.Exists;
            }
            else
            {
                ParseError(null, status);
            }
        }
    }
}