using Core.Infrastructure.Interfaces;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;
using static System.Text.Encoding;
using static System.TimeSpan;
using static System.Activator;
using System.Net.Http;
using static Newtonsoft.Json.JsonConvert;
using System.IO;
using System.IO.Compression;

namespace Core.Infrastructure.Base
{
    public  class ApiHandler<TRequest, TResponse> where TRequest : class, IProcess where TResponse : class
    {

        private readonly TimeSpan _timeOut;
        private readonly Encoding _encoding;
        private readonly string _mediaType;
        private const string EmptyContent = "[]";
        private const string Gzip = "gzip";
        private const string BaseExceptionString = "Error Occured!";
        private const string ContentNullExceptionString = "Content is null";

        [ExcludeFromCodeCoverage]
        public ApiHandler()
            : this(FromSeconds(60))
        {
        }

        [ExcludeFromCodeCoverage]
        public ApiHandler(TimeSpan timeOut)
            : this(timeOut, UTF8)
        {
        }

        [ExcludeFromCodeCoverage]
        public ApiHandler(Encoding encoding)
            : this(FromSeconds(60), encoding)
        {
        }

        [ExcludeFromCodeCoverage]
        public ApiHandler(string mediaType)
            : this(FromSeconds(60), mediaType)
        {
        }

        [ExcludeFromCodeCoverage]
        public ApiHandler(TimeSpan timeOut, Encoding encoding)
            : this(timeOut, encoding, "application/json")
        {
        }

        [ExcludeFromCodeCoverage]
        public ApiHandler(TimeSpan timeOut, string mediaType)
            : this(timeOut, UTF8, mediaType)
        {
        }

        [ExcludeFromCodeCoverage]
        public ApiHandler(Encoding encoding, string mediaType)
            : this(FromSeconds(60), encoding, mediaType)
        {
        }

        [ExcludeFromCodeCoverage]
        public ApiHandler(TimeSpan timeOut, Encoding encoding, string mediaType)
        {
            _timeOut = timeOut;
            _encoding = encoding;
            _mediaType = mediaType;
        }

        public TResponse Post(string url, TRequest request)
        {
            TResponse result = null;
            try
            {
                result = PostAsync(url, request).Result;
            }
            catch (Exception exception)
            {
                if (exception.InnerException != null) throw exception.InnerException;
            }

            return result;
        }

        public TResponse Get(string url, TRequest request)
        {
            TResponse result = null;
            try
            {
                result = GetAsync(url, request).Result;
            }
            catch (Exception exception)
            {
                if (exception.InnerException != null) throw exception.InnerException;
            }

            return result;
        }

        public async Task<TResponse> PostAsync(string url, TRequest request)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException(nameof(url));
            }

            var result = CreateInstance<TResponse>();

            using (var client = new HttpClient())
            {
                client.Timeout = FromSeconds(60);
                client.DefaultRequestHeaders.ExpectContinue = false;
                var response = await client.PostAsync(url,
                    new StringContent(SerializeObject(request), _encoding, _mediaType));
                if (response.Content.Headers.ContentEncoding.Contains(Gzip))
                {
                    var content = await response.Content.ReadAsByteArrayAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception(BaseExceptionString);
                    }

                    if (content == null)
                    {
                        throw new Exception(ContentNullExceptionString);
                    }

                    using (var inputStream = new MemoryStream(content))
                    using (var gzip = new GZipStream(inputStream, CompressionMode.Decompress))
                    using (var reader = new StreamReader(gzip))
                    {
                        var readedContent = await reader.ReadToEndAsync();
                        result = DeserializeObject<TResponse>(readedContent);
                    }
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception(content);
                    }

                    if (!string.IsNullOrWhiteSpace(content) && content != EmptyContent)
                    {
                        result = DeserializeObject<TResponse>(content);
                    }
                }
            }

            return result;
        }

        public async Task<TResponse> GetAsync(string url, TRequest request)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException(nameof(url));
            }

            var result = CreateInstance<TResponse>();
            using (var client = new HttpClient())
            {
                client.Timeout = _timeOut;
                client.DefaultRequestHeaders.ExpectContinue = false;
                var response =
                    await client.GetAsync(ApiHelper.CreateUrl(url, request, true));
                if (response.Content.Headers.ContentEncoding.Contains(Gzip))
                {
                    var content = await response.Content.ReadAsByteArrayAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception(BaseExceptionString);
                    }

                    if (content == null)
                    {
                        throw new Exception(ContentNullExceptionString);
                    }

                    using (var inputStream = new MemoryStream(content))
                    using (var gzip = new GZipStream(inputStream, CompressionMode.Decompress))
                    using (var reader = new StreamReader(gzip))
                    {
                        result = DeserializeObject<TResponse>(await reader.ReadToEndAsync());
                    }
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception(content);
                    }

                    if (!string.IsNullOrWhiteSpace(content) && content != EmptyContent)
                    {
                        result = DeserializeObject<TResponse>(content);
                    }
                }
            }

            return result;
        }
    }

}

