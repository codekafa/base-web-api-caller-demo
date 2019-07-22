using System;

namespace Core.Infrastructure.Base
{
    public static class ApiHelper
    {
        public static string CreateUrl(string url)
        {
            if (!IsValidUrl(url))
            {
                throw new UriFormatException("Invalid Uri");
            }

            return url;
        }

        public static string CreateUrl<TRequest>(string url, TRequest request, bool ignoreNullProperties = false)
        {
            if (!IsValidUrl(url))
            {
                throw new UriFormatException("Invalid Uri");
            }

            var queryString = string.Empty;
            if (request != null)
            {
                queryString = QueryGenerator.Generate(request, ignoreNullProperties);
            }

            return !string.IsNullOrWhiteSpace(queryString) ? $"{url}?{queryString}" : url;
        }

        public static bool IsValidUrl(string url) => !string.IsNullOrWhiteSpace(url) && Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute);
    }
}