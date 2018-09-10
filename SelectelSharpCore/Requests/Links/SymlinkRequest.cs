using System.Net.Http;
using SelectelSharpCore.Models.Link;
using SelectelSharpCore.Requests.Container;

namespace SelectelSharpCore.Requests.Links
{
    public class SymlinkRequest : ContainerRequest<bool>
    {
        private readonly string link;

        internal override HttpMethod Method => HttpMethod.Put;

        public SymlinkRequest(string containerName, Symlink link)
            : base(containerName)
        {
            this.link = link.Link;
            SetCustomHeaders(link.GetHeaders());
        }

        protected override string GetUrl(string storageUrl)
        {
            return string.Format("{0}/{1}/{2}", storageUrl, ContainerName, link);
        }
    }
}