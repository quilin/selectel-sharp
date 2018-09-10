using SelectelSharpCore.Common;
using SelectelSharpCore.Headers;
using System;
using System.Collections.Generic;

namespace SelectelSharpCore.Models.Link
{
    public class Symlink
    {
        public string ContentType { get; private set; }
        public long? DeleteAt { get; private set; }
        public string Key { get; private set; }
        public string ObjectLocation { get; private set; }
        public string Link { get; private set; }
        public string ContentDisposition { get; private set; }

        public Symlink(string link, SymlinkType type, string objectLocaton, DateTime? deleteAt = null,
            string password = null, string contentDisposition = null)
        {
            if (string.IsNullOrEmpty(link))
                throw new ArgumentNullException(nameof(link));

            if (objectLocaton == null)
                throw new ArgumentNullException(nameof(objectLocaton));

            if (string.IsNullOrWhiteSpace(objectLocaton))
                throw new ArgumentOutOfRangeException(nameof(link));

            Link = link;
            ContentType = typeToString[type];
            ContentDisposition = contentDisposition;
            ObjectLocation = Uri.EscapeUriString(objectLocaton);

            if (password != null)
            {
                Key = Helpers.CalculateSha1(string.Concat(password, ObjectLocation));
            }

            if (deleteAt.HasValue)
            {
                DeleteAt = Helpers.DateToUnixTimestamp(deleteAt.Value);
            }
        }

        public IDictionary<string, object> GetHeaders()
        {
            var result = new Dictionary<string, object>();

            TryAddHeader(result, HeaderKeys.ContentType, ContentType);
            TryAddHeader(result, HeaderKeys.ContentLength, 0);
            TryAddHeader(result, HeaderKeys.ContentDisposition, ContentDisposition);
            TryAddHeader(result, HeaderKeys.XObjectMetaLocation, ObjectLocation);
            TryAddHeader(result, HeaderKeys.XObjectMetaDeleteAt, DeleteAt);
            TryAddHeader(result, HeaderKeys.XObjectMetaLinkKey, Key);

            return result;
        }

        private void TryAddHeader(Dictionary<string, object> headers, string header, object value)
        {
            if (value != null)
            {
                headers.Add(header, value.ToString());
            }
        }

        private static Dictionary<SymlinkType, string> typeToString = new Dictionary<SymlinkType, string>
        {
            {SymlinkType.Symlink, "x-storage/symlink"},
            {SymlinkType.OnetimeSymlink, "x-storage/onetime-symlink"},
            {SymlinkType.SymlinkSecure, "x-storage/symlink+secure"},
            {SymlinkType.OnetimeSymlinkSecure, "x-storage/onetime-symlink+secure"},
        };

        public enum SymlinkType
        {
            /// <summary>
            /// обычная ссылка
            /// </summary>
            Symlink,

            /// <summary>
            /// Одноразовая ссылка
            /// </summary>
            OnetimeSymlink,

            /// <summary>
            /// Обычная запароленная ссылка
            /// </summary>
            SymlinkSecure,

            /// <summary>
            /// Одноразовая запароленная ссылка
            /// </summary>
            OnetimeSymlinkSecure
        }
    }
}