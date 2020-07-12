using AIS.Application.Interfaces.Infrastructure;
using AIS.Application.PictureSearchers.Models;
using System.Threading;
using System.Threading.Tasks;

namespace AIS.Application.PictureSearchers
{
    public class IqdbSearchExecutor : ISearchExecutor
    {
        private readonly IIqdbWebClient _iqdbWebClient;
        private readonly IIqdbResponseParser _iqdbResponseParser;
        public async Task Search(
            IqdbSearchFileRequest iqdbFileSearch,
            CancellationToken token = default)
        {
            // делаем запрос к Iqdb
            var file = iqdbFileSearch.LocalImage;
            var webPageStream = await _iqdbWebClient.RequestImageSearch(file, token);
            var parseResults = _iqdbResponseParser.ParseResponse(webPageStream, token);

            // используя конфиг запроса поиска фильтруем результаты

            // возвращаем результат
        }
    }
}
