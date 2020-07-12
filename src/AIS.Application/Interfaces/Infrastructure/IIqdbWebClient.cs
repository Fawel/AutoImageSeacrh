using AIS.Application.PictureSearchers.Models;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AIS.Application.Interfaces.Infrastructure
{
    public interface IIqdbWebClient
    {
        Task<StreamReader> RequestImageSearch(LocalImageInfo filePath,
                                              CancellationToken token = default);
    }
}
