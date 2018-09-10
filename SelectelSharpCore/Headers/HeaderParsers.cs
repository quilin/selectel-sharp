using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;

namespace SelectelSharpCore.Headers
{
    internal static class HeaderParsers
    {
        internal static int ToInt(string value)
        {
            return int.TryParse(value, out var result) ? result : 0;
        }

        internal static long ToLong(string value)
        {
            return long.TryParse(value, out var result) ? result : 0;
        }

        internal static T ParseHeaders<T>(HttpResponseHeaders headers)
            where T : new()
        {
            var obj = new T();
            ParseHeaders(obj, headers);
            return obj;
        }

        internal static void ParseHeaders(object obj, HttpResponseHeaders headers)
        {
            var props = obj
                .GetType()
                .GetRuntimeProperties();

            foreach (var prop in props)
            {
                var headerAttr = GetAttribute<HeaderAttribute>(prop);
                if (headerAttr == null) continue;
                if (headerAttr.CustomHeaders)
                {
                    var customHeadersKeys = headers
                        .Where(x => x.Key.ToLower().StartsWith(HeaderKeys.XContainerMetaPrefix.ToLower()))
                        .Where(x => !string.Equals(x.Key, HeaderKeys.XContainerMetaType,
                            StringComparison.CurrentCultureIgnoreCase))
                        .Where(x => !string.Equals(x.Key, HeaderKeys.XContainerMetaGallerySecret,
                            StringComparison.CurrentCultureIgnoreCase))
                        .Select(x => x.Key)
                        .ToArray();

                    if (!customHeadersKeys.Any()) continue;
                    var customHeaders =
                        customHeadersKeys.ToDictionary(key => key, key => headers.GetValues(key).FirstOrDefault());
                    prop.SetValue(obj, customHeaders);
                }
                else if (headerAttr.CorsHeaders)
                {
                    var cors = new CorsHeaders(headers);
                    prop.SetValue(obj, cors);
                }
                else
                {
                    if (!headers.TryGetValues(headerAttr.HeaderKey, out var values))
                        continue;
                    var value = values.FirstOrDefault();
                    if (value == null)
                        continue;

                    if (value.GetType() == prop.PropertyType)
                    {
                        prop.SetValue(obj, value);
                    }
                    else
                    {
                        try
                        {
                            var convertedValue = Convert.ChangeType(value, prop.PropertyType);
                            prop.SetValue(obj, convertedValue);
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                }
            }
        }

        private static T GetAttribute<T>(MemberInfo pi)
            where T : Attribute => pi.GetCustomAttributes<T>().FirstOrDefault();
    }
}