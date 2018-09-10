using SelectelSharpCore.Models.Container;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using SelectelSharpCore.Exceptions;

namespace SelectelSharpCore.Requests.Container
{
    /// <summary>
    /// Запрос информации по хранилищу со списком контейнеров
    /// </summary>
    public class GetContainersRequest : BaseRequest<ContainersList>
    {
        internal override HttpMethod Method => HttpMethod.Get;

        /// <param name="limit">Число, ограничивает количество объектов в результате (по умолчанию 10000)</param>
        /// <param name="marker">Cтрока, результат будет содержать объекты по значению больше указанного маркера (полезно использовать для постраничной навигации и при большом количестве контейнеров)</param>
        public GetContainersRequest(int limit = 10000, string marker = null)
        {
            TryAddQueryParam("limit", limit);
            TryAddQueryParam("marker", marker);
            TryAddQueryParam("format", "json");
        }

        internal override void Parse(HttpResponseHeaders headers, object content, HttpStatusCode status)
        {
            if (status == HttpStatusCode.OK)
            {
                base.Parse(headers, content, status);
            }
            else if (status == HttpStatusCode.NoContent)
            {
                Result = null;
            }
            else
            {
                throw new SelectelWebException(status);
            }
        }
    }
}