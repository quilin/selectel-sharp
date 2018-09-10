using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SelectelSharpCore.Headers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SelectelSharpCore.Exceptions;
using SelectelSharpCore.HttpContents;

namespace SelectelSharpCore.Requests
{
    public abstract class BaseRequest<T>
    {
        public byte[] File { get; protected set; }
        public Func<long, Task> Progress { get; protected set; }
        public T Result { get; protected set; }
        public bool IsSuccessful { get; set; }

        public virtual bool AllowAnonymously => false;

        public virtual bool DownloadData => false;

        internal virtual HttpMethod Method => HttpMethod.Get;

        private readonly IDictionary<string, string> query = new Dictionary<string, string>();
        private readonly IDictionary<string, string> headers = new Dictionary<string, string>();

        protected void SetCustomHeaders(IDictionary<string, object> inputHeaders = null)
        {
            if (inputHeaders == null)
            {
                return;
            }

            foreach (var header in inputHeaders)
            {
                TryAddHeader(header.Key, header.Value);
            }
        }

        protected void SetCorsHeaders(CorsHeaders cors = null)
        {
            if (cors == null)
            {
                return;
            }

            foreach (var header in cors.GetHeaders())
            {
                TryAddHeader(header.Key, header.Value);
            }
        }

        protected void SetConditionalHeaders(ConditionalHeaders conditional = null)
        {
            if (conditional == null)
            {
                return;
            }

            foreach (var header in conditional.GetHeaders())
            {
                TryAddHeader(header.Key, header.Value);
            }
        }

        protected virtual string GetUrl(string storageUrl)
        {
            return storageUrl;
        }

        private Uri GetUri(string storageUrl)
        {
            var url = GetUrl(storageUrl);

            if (query == null || !query.Any()) return new Uri(url);

            var queryParamsList = query
                .Where(x => !string.IsNullOrEmpty(x.Value))
                .Select(x => string.Concat(x.Key, "=", x.Value));
            var queryParams = string.Join("&", queryParamsList);
            return new Uri(url.Contains("?")
                ? string.Concat(url, queryParams)
                : string.Concat(url, "?", queryParams));
        }

        internal void TryAddQueryParam(string key, object value)
        {
            if (value != null)
            {
                query.Add(key, value.ToString());
            }
        }

        internal void TryAddHeader(string key, object value)
        {
            if (value != null)
            {
                headers.Add(key, value.ToString());
            }
        }

        internal virtual async Task Execute(string storageUrl, string authToken)
        {
            var uri = GetUri(storageUrl);

#if DEBUG
            Debug.WriteLine(uri.ToString());
#endif

            //var client = new HttpClient();

            var request = new HttpRequestMessage
            {
                RequestUri = uri,
                Method = Method,
            };

            //var request = HttpWebRequest.CreateHttp(uri);
            //request.Method = this.Method.ToString();

            if (!AllowAnonymously)
            {
                TryAddHeader(HeaderKeys.XAuthToken, authToken);
            }

            // set Accept header
            if (headers.ContainsKey(HeaderKeys.Accept))
            {
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(headers[HeaderKeys.Accept]));
                headers.Remove(HeaderKeys.Accept);
            }

            // set Content-Type header
            if (headers.ContainsKey(HeaderKeys.ContentType))
            {
                request.Content.Headers.ContentType = new MediaTypeHeaderValue(headers[HeaderKeys.ContentType]);
                headers.Remove(HeaderKeys.ContentType);
            }

            // set Content-Length header
            // todo: make SetContLength method
            if (headers.ContainsKey(HeaderKeys.ContentLength))
            {
                request.Content.Headers.ContentLength = long.Parse(headers[HeaderKeys.ContentLength]);
                //request.Headers.TryAddWithoutValidation(HeaderKeys.ContentLength, File.Length.ToString());

                headers.Remove(HeaderKeys.ContentLength);
            }

            // set custom headers
            if (headers != null)
            {
                foreach (var kv in headers.Where(x => !string.IsNullOrEmpty(x.Value)))
                {
                    request.Headers.Add(kv.Key, kv.Value);
                }
            }

            if (File != null && File.Length > 0)
            {
                request.Content = Progress == null
                    ? (HttpContent) new ByteArrayContent(File)
                    : new UploadProgressContent(File, Progress);
            }

            var status = HttpStatusCode.OK;
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.SendAsync(request);
                    status = response.StatusCode;
                    response.EnsureSuccessStatusCode();

                    using (var rs = await response.Content.ReadAsStreamAsync())
                    {
                        if (DownloadData)
                        {
                            ParseData(response, status, rs);
                        }
                        else
                        {
                            ParseString(response, status, rs);
                        }
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                ParseError(ex, status);
            }
        }

        private void ParseString(HttpResponseMessage response, HttpStatusCode status, Stream rs)
        {
            using (var reader = new StreamReader(rs, Encoding.UTF8))
            {
                var content = reader.ReadToEnd();
                Parse(response.Headers, content, status);
            }
        }

        private void ParseData(HttpResponseMessage response, HttpStatusCode status, Stream rs)
        {
            using (var ms = new MemoryStream())
            {
                rs.CopyTo(ms);
                Parse(response.Headers, ms.ToArray(), status);
            }
        }

        internal virtual void Parse(HttpResponseHeaders inputHeaders, object content, HttpStatusCode status)
        {
            if (content != null)
            {
                SetResult(content);
            }
            else
            {
                SetStatusResult(status);
            }
        }

        internal virtual void ParseError(HttpRequestException ex, HttpStatusCode status)
        {
            if (ex != null)
            {
                throw ex;
            }

            throw new SelectelWebException(status);
        }

        internal virtual void SetStatusResult(HttpStatusCode status, params HttpStatusCode[] successfulStatuses)
        {
            if (typeof(T).GetTypeInfo().IsEnum && typeof(T) == status.GetType())
            {
                if (successfulStatuses == null || !successfulStatuses.Any() || successfulStatuses.Contains(status))
                {
                    SetResult(status);
                }
                else
                {
                    throw new SelectelWebException(status);
                }
            }
            else
            {
                throw new ArgumentException();
            }
        }

        private void SetResult(object content)
        {
            switch (content)
            {
                case T result:
                    Result = result;
                    IsSuccessful = true;
                    break;
                case string s:
                    Result = JsonConvert.DeserializeObject<T>(s,
                        new IsoDateTimeConverter {DateTimeFormat = "YYYY-MM-DDThh:mm:ss.sTZD"});
                    IsSuccessful = true;
                    break;
                default:
                    throw new ArgumentException();
            }
        }
    }
}