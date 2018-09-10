using SelectelSharpCore.Common;
using SelectelSharpCore.Headers;
using SelectelSharpCore.Models.Container;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SelectelSharpCore.Requests.Container
{
    /// <summary>
    /// Запрос на преобразовать в галерею для удобного публичного представления в веб-браузере загруженных в нем фотографий и других изображений.
    /// </summary>
    public class UpdateContainerToGalleryRequest : ContainerRequest<UpdateContainerResult>
    {
        /// <param name="containerName">Имя контейнера должно быть меньше 256 символов и не содержать завершающего слеша '/' в конце.</param>
        /// <param name="password">Дополнительно можно установить пароль, по которому будет ограничен доступ.</param>
        public UpdateContainerToGalleryRequest(string containerName, string password = null)
            : base(containerName)
        {
            if (string.IsNullOrEmpty(password) == false)
            {
                TryAddHeader(HeaderKeys.XContainerMetaGallerySecret, Helpers.CalculateSha1(password));
            }

            TryAddHeader(HeaderKeys.XContainerMetaType, ContainerType.Gallery.ToString().ToLower());
        }

        internal override HttpMethod Method => HttpMethod.Post;

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