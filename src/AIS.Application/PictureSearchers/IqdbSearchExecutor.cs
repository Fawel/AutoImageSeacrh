using AIS.Application.Interfaces.Infrastructure;
using AIS.Domain.PictureSearhers;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AIS.Application.PictureSearchers
{
    public class IqdbSearchExecutor : ISearchExecutor
    {
        private readonly IIqdbWebClient _iqdbWebClient;
        private readonly IFileSystem _fileSystem;
        public async Task<IqdbSearchResult> Search(string filePath, CancellationToken token = default)
        {
            await _fileSystem.File.ReadAllBytesAsync(filePath, token);
            //await _iqdbWebClient.PostMessage(token);

            throw new NotImplementedException();
        }
    }
}
