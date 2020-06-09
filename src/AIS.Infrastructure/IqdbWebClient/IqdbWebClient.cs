using AIS.Application.Interfaces.Infrastructure;
using AIS.Application.PictureSearchers.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace AIS.Infrastructure.IqdbWebClient
{
    public class IqdbWebClient : IIqdbWebClient
    {
        private readonly HttpClient _httpClient;

        public IqdbWebClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<StreamReader> RequestImageSearch(ImageInfo image,
                                                           IqdbSearchSettings searchSettings = default,
                                                           CancellationToken token = default)
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            using var content = new MultipartFormDataContent(boundary);
            using var fileStream = new StreamReader(image.FilePath).BaseStream;
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                FileName = image.Name,
                Name = "file",
                Size = fileStream.Length,
            };

            fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            content.Add(fileContent);

            var requestUri = "http://iqdb.org/";
            var result = await _httpClient.PostAsync(requestUri, content);
            StreamReader htmlResponseStream = new StreamReader(await result.Content.ReadAsStreamAsync());
            return htmlResponseStream;
        }
    }
}
