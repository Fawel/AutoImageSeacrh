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
        public async Task SearchByIqdb(int imageToSearchId, 
            CancellationToken token = default)
        {
            await _searchRepository.IsPictureSearched(imageToSearchId);
        }
    }
}
