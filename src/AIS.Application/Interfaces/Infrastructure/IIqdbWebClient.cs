using AIS.Application.PictureSearchers.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AIS.Application.Interfaces.Infrastructure
{
    public interface IIqdbWebClient
    {
        Task<StreamReader> RequestImageSearch(ImageInfo filePath,
                                              IqdbSearchSettings searchSettings = default,
                                              CancellationToken token = default);
    }
}
