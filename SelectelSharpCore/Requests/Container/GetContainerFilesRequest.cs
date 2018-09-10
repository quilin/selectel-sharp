using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using SelectelSharpCore.Exceptions;
using SelectelSharpCore.Models.Container;
using SelectelSharpCore.Models;

namespace SelectelSharpCore.Requests.Container
{
    public class GetContainerFilesRequest : ContainerRequest<ContainerFilesList>
    {
        /// <summary>
        /// Запрос на получение списка файлов в контейнере
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="limit">Число, ограничивает количество объектов в результате (по умолчанию 10000)</param>
        /// <param name="marker">Cтрока, результат будет содержать объекты по значению больше указанного маркера (полезно использовать для постраничной навигации и при большом количестве контейнеров)</param>        
        /// <param name="prefix">Строка, вернуть объекты имена которых начинаются с указанного префикса</param>
        /// <param name="path">Строка, вернуть объекты в указанной папке(виртуальные папки)</param>
        /// <param name="delimeter">Символ, вернуть объекты до указанного разделителя в их имени</param>
        public GetContainerFilesRequest(
            string containerName,
            int limit = 10000,
            string marker = null,
            string prefix = null,
            string path = null,
            string delimeter = null) :
            base(containerName)
        {
            TryAddQueryParam("limit", limit);
            TryAddQueryParam("marker", marker);
            TryAddQueryParam("prefix", prefix);
            TryAddQueryParam("path", path);
            TryAddQueryParam("delimeter", delimeter);
            TryAddQueryParam("format", "json");
        }

        internal override void Parse(HttpResponseHeaders headers, object content, HttpStatusCode status)
        {
            if (status == HttpStatusCode.OK)
            {
                base.Parse(headers, content, status);
                Result.StorageInfo = new StorageInfo(headers);
            }
            else
            {
                throw new SelectelWebException(status);
            }
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