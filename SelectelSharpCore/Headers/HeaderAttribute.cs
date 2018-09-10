using System;

namespace SelectelSharpCore.Headers
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class HeaderAttribute : Attribute
    {
        public HeaderAttribute(string headerKey = null)
        {
            HeaderKey = headerKey;
        }

        public string HeaderKey { get; }

        public bool CustomHeaders { get; set; }
        public bool CorsHeaders { get; set; }
    }
}