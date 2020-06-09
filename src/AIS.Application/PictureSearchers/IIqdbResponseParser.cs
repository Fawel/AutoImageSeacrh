using AIS.Application.PictureSearchers.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AIS.Application.PictureSearchers
{
    public interface IIqdbResponseParser
    {
        IqdbSearchResponse ParseResponse(StreamReader iqdbResponseStream, CancellationToken token = default);
    }
}
