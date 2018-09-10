using SelectelSharpCore.Headers;
using SelectelSharpCore.Models.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SelectelSharpCore.Requests.File
{
    public class UploadFileRequest : FileRequest<UploadFileResult>
    {
        private string ETag { get; set; }

        public UploadFileRequest(
            string containerName,
            string fileName,
            byte[] file,
            Func<long, Task> progress = null,
            bool validateChecksum = false,
            string contentDisposition = null,
            string contentType = null,
            long? deleteAt = null,
            long? deleteAfter = null,
            IDictionary<string, object> customHeaders = null) : base(containerName, fileName)
        {
            TryAddHeader(HeaderKeys.ContentDisposition, contentDisposition);

            if (deleteAfter.HasValue)
            {
                TryAddHeader(HeaderKeys.XDeleteAfter, deleteAfter.Value);
            }

            if (deleteAt.HasValue)
            {
                TryAddHeader(HeaderKeys.XDeleteAt, deleteAt.Value);
            }

            if (string.IsNullOrEmpty(contentType))
            {
                TryAddHeader(HeaderKeys.ContentType, contentType);
            }

            if (string.IsNullOrEmpty(contentDisposition))
            {
                TryAddHeader(HeaderKeys.ContentDisposition, contentDisposition);
            }

            SetCustomHeaders(customHeaders);

            if (validateChecksum)
            {
                ETag = GetETag(file);
                TryAddHeader(HeaderKeys.ETag, ETag);
            }

            File = file;
            Progress = progress;
        }

        internal override HttpMethod Method => HttpMethod.Put;

        internal override void Parse(HttpResponseHeaders headers, object data, HttpStatusCode status)
        {
            if (status == HttpStatusCode.Created)
            {
                if (ETag != null)
                {
                    // idk why Selectel's ETag check not working, so check the result once again on client.
                    if (headers.GetValues(HeaderKeys.ETag).FirstOrDefault()
                        .Equals(ETag, StringComparison.OrdinalIgnoreCase) == false)
                    {
                        Result = UploadFileResult.CheckSumValidationFailed;
                        return;
                    }
                }

                Result = UploadFileResult.Created;
            }
            else if ((int) status == 422)
            {
                Result = UploadFileResult.CheckSumValidationFailed;
            }
            else
            {
                ParseError(null, status);
            }
        }

        private string GetETag(byte[] file)
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(file);
                return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
                //return Encoding.Default.GetString();
            }
        }
    }
}