using AIS.Application.PictureSearchers.Models;
using System.IO;
using System.Threading;

namespace AIS.Application.Interfaces.Infrastructure
{
    public interface IIqdbResponseParser
    {
        IqdbSearchResponse ParseResponse(StreamReader iqdbResponseStream, CancellationToken token = default);
    }
}
