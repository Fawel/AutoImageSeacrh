using AIS.Application.Interfaces.Repositories;
using AIS.Domain.PictureSearhers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AIS.Application.PictureSearchers
{
    public class PictureSearchService
    {
        private readonly ISearchRepository _searchRepository;
        private readonly IqdbSearchExecutor _iqdbSearchExecutor;
        public async Task<PictureSearch> InitNewSearch(int imageToSearchId, 
            Searcher searcher, 
            CancellationToken token = default)
        {
            var imagePath = await _searchRepository.GetPictureFile(imageToSearchId);
            if (!(searcher is IqdbSearcher))
                throw new NotImplementedException();

            var searchResult = await _iqdbSearchExecutor.Search(imagePath, token);
            if (!searchResult.IsFound)
                throw new NotImplementedException();

            var newSearch = new PictureSearch();
            newSearch.AddSearcher(searcher);
            newSearch.AddPictureToSearch(imageToSearchId);
            return null;
        }
    }
}
