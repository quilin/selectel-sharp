using SelectelSharpCore.Headers;
using System.Net.Http.Headers;

namespace SelectelSharpCore.Models.File
{
    public class GetFileResult : FileInfo
    {
        public byte[] File { get; set; }

        public GetFileResult(byte[] file, string name, HttpResponseHeaders headers)
        {
            HeaderParsers.ParseHeaders(this, headers);
            File = file;
            Name = name;
            Bytes = file.Length;
        }
    }
}