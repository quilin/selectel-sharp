using SelectelSharpCore.Requests.Container;
using System;
using System.Text;

namespace SelectelSharpCore.Requests.File
{
    public class FileRequest<T> : ContainerRequest<T>
    {
        private const int MaxFileNameSize = 1024;

        protected string Path;
        protected string FileName;


        public FileRequest(string container, string path)
            : base(container)
        {
            var parts = path.Split('/');
            if (parts.Length > 1)
            {
                FileName = parts[parts.Length - 1];
            }
            else
            {
                FileName = path;
            }

            path = Uri.EscapeUriString(path);
            if (Encoding.UTF8.GetByteCount(path) > MaxFileNameSize)
            {
                throw new Exception(
                    "Полное имя файла (включая виртуальные папки) не должно превышать 1024 байт после URL квотирования.");
            }

            Path = path;
        }

        protected override string GetUrl(string storageUrl)
        {
            return string.Format("{0}/{1}/{2}", storageUrl, ContainerName, Path);
        }
    }
}